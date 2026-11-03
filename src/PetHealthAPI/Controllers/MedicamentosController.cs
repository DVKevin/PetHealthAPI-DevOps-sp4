using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Aplicacao.Interfaces;
using PetHealthAPI.Dominio.Constantes;
using PetHealthAPI.Dominio.Filtros;
using PetHealthAPI.Hateoas;

namespace PetHealthAPI.Controllers;

/// <summary>
/// Registro e controle de medicamentos dos pets.
/// </summary>
[Route("api/[controller]")]
public class MedicamentosController : ApiControllerBase
{
    private readonly IMedicamentoServico _medicamentoServico;

    public MedicamentosController(IMedicamentoServico medicamentoServico)
    {
        _medicamentoServico = medicamentoServico;
    }

    /// <summary>
    /// Lista medicamentos com paginação, filtros (petId, nome, ativo)
    /// e ordenação (nome, dataInicio, dataFim).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginaComLinks<MedicamentoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginaComLinks<MedicamentoResponse>>> Listar([FromQuery] FiltroMedicamento filtro)
    {
        var resultado = await _medicamentoServico.ListarAsync(filtro);
        return Ok(CriarPagina(resultado, LinksDoMedicamento));
    }

    /// <summary>Busca um medicamento pelo ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RecursoComLinks<MedicamentoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecursoComLinks<MedicamentoResponse>>> ObterPorId(int id)
    {
        var medicamento = await _medicamentoServico.ObterPorIdAsync(id);
        return Ok(ComLinks(medicamento, LinksDoMedicamento(medicamento)));
    }

    /// <summary>Registra um medicamento para um pet existente (nasce ativo).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(RecursoComLinks<MedicamentoResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecursoComLinks<MedicamentoResponse>>> Criar([FromBody] CriarMedicamentoRequest request)
    {
        var medicamento = await _medicamentoServico.CriarAsync(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = medicamento.Id },
                               ComLinks(medicamento, LinksDoMedicamento(medicamento)));
    }

    /// <summary>Atualiza os dados de um medicamento.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarMedicamentoRequest request)
    {
        await _medicamentoServico.AtualizarAsync(id, request);
        return NoContent();
    }

    /// <summary>Encerra o tratamento (se a data de fim não for enviada, usa a data atual).</summary>
    [HttpPatch("{id:int}/encerrar")]
    [ProducesResponseType(typeof(RecursoComLinks<MedicamentoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecursoComLinks<MedicamentoResponse>>> Encerrar(int id,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] EncerrarMedicamentoRequest? request)
    {
        var medicamento = await _medicamentoServico.EncerrarAsync(id, request ?? new EncerrarMedicamentoRequest(null));
        return Ok(ComLinks(medicamento, LinksDoMedicamento(medicamento)));
    }

    /// <summary>Remove o registro de um medicamento.</summary>
    /// <remarks>Somente o perfil Admin.</remarks>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = PerfisUsuario.Admin)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id)
    {
        await _medicamentoServico.RemoverAsync(id);
        return NoContent();
    }

    private IEnumerable<Link> LinksDoMedicamento(MedicamentoResponse medicamento)
    {
        var links = new List<Link>
        {
            CriarLink(nameof(ObterPorId), "self", MetodoGet, new { id = medicamento.Id }),
            CriarLink(nameof(Atualizar), "atualizar", MetodoPut, new { id = medicamento.Id }),
            CriarLink(nameof(Remover), "remover", MetodoDelete, new { id = medicamento.Id }),
            CriarLink(nameof(PetsController.ObterPorId), "pet", MetodoGet, new { id = medicamento.PetId }, NomesControllers.Pets)
        };

        // HATEOAS orientado a estado: só oferece "encerrar" enquanto o tratamento estiver ativo
        if (medicamento.Ativo)
            links.Add(CriarLink(nameof(Encerrar), "encerrar", MetodoPatch, new { id = medicamento.Id }));

        return links;
    }
}
