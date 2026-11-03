using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Aplicacao.Interfaces;
using PetHealthAPI.Dominio.Constantes;
using PetHealthAPI.Dominio.Filtros;
using PetHealthAPI.Hateoas;
using PetHealthAPI.Infraestrutura.Observabilidade;

namespace PetHealthAPI.Controllers;

/// <summary>
/// Gerenciamento dos pets cadastrados na plataforma.
/// </summary>
[Route("api/[controller]")]
public class PetsController : ApiControllerBase
{
    private readonly IPetServico _petServico;
    private readonly AplicacaoMetricas _metricas;

    public PetsController(IPetServico petServico, AplicacaoMetricas metricas)
    {
        _petServico = petServico;
        _metricas = metricas;
    }

    /// <summary>
    /// Lista pets com paginação, filtros (nome, especie, raca, castrado, tutorId)
    /// e ordenação (nome, especie, idade, peso, dataCadastro).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginaComLinks<PetResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginaComLinks<PetResponse>>> Listar([FromQuery] FiltroPet filtro)
    {
        var resultado = await _petServico.ListarAsync(filtro);
        return Ok(CriarPagina(resultado, LinksDoPet));
    }

    /// <summary>Busca um pet pelo ID, com links para o histórico de saúde.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RecursoComLinks<PetResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecursoComLinks<PetResponse>>> ObterPorId(int id)
    {
        var pet = await _petServico.ObterPorIdAsync(id);
        return Ok(ComLinks(pet, LinksDoPet(pet)));
    }

    /// <summary>Cadastra um novo pet vinculado a um tutor existente.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(RecursoComLinks<PetResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecursoComLinks<PetResponse>>> Criar([FromBody] PetRequest request)
    {
        using var atividade = AplicacaoMetricas.ActivitySource.StartActivity("CriarPetEndpoint");
        atividade?.SetTag("pet.nome", request.Nome);
        atividade?.SetTag("pet.tutorId", request.TutorId);

        try
        {
            var pet = await _petServico.CriarAsync(request);

            _metricas.RegistrarPetCriado("sucesso");
            atividade?.SetTag("pet.id", pet.Id);

            return CreatedAtAction(nameof(ObterPorId), new { id = pet.Id }, ComLinks(pet, LinksDoPet(pet)));
        }
        catch (ArgumentException ex)
        {
            _metricas.RegistrarPetCriado("erro_validacao");
            atividade?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw; // o ManipuladorExcecoesGlobal transforma em 400 (ProblemDetails)
        }
    }

    /// <summary>Atualiza os dados de um pet.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] PetRequest request)
    {
        await _petServico.AtualizarAsync(id, request);
        return NoContent();
    }

    /// <summary>Remove um pet (e, em cascata, vacinas, consultas e medicamentos).</summary>
    /// <remarks>Somente o perfil Admin.</remarks>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = PerfisUsuario.Admin)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id)
    {
        await _petServico.RemoverAsync(id);
        return NoContent();
    }

    private IEnumerable<Link> LinksDoPet(PetResponse pet) => new[]
    {
        CriarLink(nameof(ObterPorId), "self", MetodoGet, new { id = pet.Id }),
        CriarLink(nameof(Atualizar), "atualizar", MetodoPut, new { id = pet.Id }),
        CriarLink(nameof(Remover), "remover", MetodoDelete, new { id = pet.Id }),
        CriarLink(nameof(TutoresController.ObterPorId), "tutor", MetodoGet, new { id = pet.TutorId }, NomesControllers.Tutores),
        CriarLink(nameof(VacinasController.Listar), "vacinas", MetodoGet, new { petId = pet.Id }, NomesControllers.Vacinas),
        CriarLink(nameof(ConsultasController.Listar), "consultas", MetodoGet, new { petId = pet.Id }, NomesControllers.Consultas),
        CriarLink(nameof(MedicamentosController.Listar), "medicamentos", MetodoGet, new { petId = pet.Id }, NomesControllers.Medicamentos),
        CriarLink(nameof(RegistrosDiariosController.Listar), "registros-diarios", MetodoGet, new { petId = pet.Id }, NomesControllers.RegistrosDiarios),
        CriarLink(nameof(RegistrosDiariosController.ObterPontos), "pontos", MetodoGet, new { petId = pet.Id }, NomesControllers.RegistrosDiarios)
    };
}
