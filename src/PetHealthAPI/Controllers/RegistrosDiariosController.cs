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
/// Diário de cuidados (check-in gamificado) persistido no MongoDB.
/// Cada cuidado do dia (alimentação, hidratação, passeio, medicação) vale 10 pontos de XP.
/// </summary>
[Route("api/registros-diarios")]
public class RegistrosDiariosController : ApiControllerBase
{
    private readonly IRegistroDiarioServico _registroServico;
    private readonly AplicacaoMetricas _metricas;

    public RegistrosDiariosController(IRegistroDiarioServico registroServico, AplicacaoMetricas metricas)
    {
        _registroServico = registroServico;
        _metricas = metricas;
    }

    /// <summary>
    /// Lista registros com paginação, filtros (petId, dataInicio, dataFim) e ordenação (data, pontos).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginaComLinks<RegistroDiarioResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginaComLinks<RegistroDiarioResponse>>> Listar([FromQuery] FiltroRegistroDiario filtro)
    {
        var resultado = await _registroServico.ListarAsync(filtro);
        return Ok(CriarPagina(resultado, LinksDoRegistro));
    }

    /// <summary>Busca um registro pelo ID (ObjectId do MongoDB, 24 caracteres).</summary>
    [HttpGet("{id:length(24)}")]
    [ProducesResponseType(typeof(RecursoComLinks<RegistroDiarioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecursoComLinks<RegistroDiarioResponse>>> ObterPorId(string id)
    {
        var registro = await _registroServico.ObterPorIdAsync(id);
        return Ok(ComLinks(registro, LinksDoRegistro(registro)));
    }

    /// <summary>Total de pontos (XP) e nível do pet, calculados por agregação no MongoDB.</summary>
    [HttpGet("pets/{petId:int}/pontos")]
    [ProducesResponseType(typeof(RecursoComLinks<ResumoPontosResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecursoComLinks<ResumoPontosResponse>>> ObterPontos(int petId)
    {
        var resumo = await _registroServico.ObterResumoPontosAsync(petId);

        return Ok(ComLinks(resumo, new[]
        {
            CriarLink(nameof(ObterPontos), "self", MetodoGet, new { petId }),
            CriarLink(nameof(Listar), "registros-diarios", MetodoGet, new { petId }),
            CriarLink(nameof(PetsController.ObterPorId), "pet", MetodoGet, new { id = petId }, NomesControllers.Pets)
        }));
    }

    /// <summary>Registra o check-in diário de cuidados de um pet.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(RecursoComLinks<RegistroDiarioResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecursoComLinks<RegistroDiarioResponse>>> Criar([FromBody] CriarRegistroDiarioRequest request)
    {
        try
        {
            var registro = await _registroServico.CriarAsync(request);
            _metricas.RegistrarRegistroDiarioCriado("sucesso");

            return CreatedAtAction(nameof(ObterPorId), new { id = registro.Id }, ComLinks(registro, LinksDoRegistro(registro)));
        }
        catch (ArgumentException)
        {
            _metricas.RegistrarRegistroDiarioCriado("erro_validacao");
            throw;
        }
    }

    /// <summary>Atualiza um registro diário (os pontos são recalculados).</summary>
    [HttpPut("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(string id, [FromBody] AtualizarRegistroDiarioRequest request)
    {
        await _registroServico.AtualizarAsync(id, request);
        return NoContent();
    }

    /// <summary>Remove um registro diário.</summary>
    /// <remarks>Somente o perfil Admin.</remarks>
    [HttpDelete("{id:length(24)}")]
    [Authorize(Roles = PerfisUsuario.Admin)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(string id)
    {
        await _registroServico.RemoverAsync(id);
        return NoContent();
    }

    private IEnumerable<Link> LinksDoRegistro(RegistroDiarioResponse registro) => new[]
    {
        CriarLink(nameof(ObterPorId), "self", MetodoGet, new { id = registro.Id }),
        CriarLink(nameof(Atualizar), "atualizar", MetodoPut, new { id = registro.Id }),
        CriarLink(nameof(Remover), "remover", MetodoDelete, new { id = registro.Id }),
        CriarLink(nameof(ObterPontos), "pontos-do-pet", MetodoGet, new { petId = registro.PetId }),
        CriarLink(nameof(PetsController.ObterPorId), "pet", MetodoGet, new { id = registro.PetId }, NomesControllers.Pets)
    };
}
