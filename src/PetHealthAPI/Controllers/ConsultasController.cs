using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Aplicacao.Interfaces;
using PetHealthAPI.Dominio.Constantes;
using PetHealthAPI.Dominio.Filtros;
using PetHealthAPI.Hateoas;

namespace PetHealthAPI.Controllers;

/// <summary>
/// Histórico clínico: consultas veterinárias dos pets.
/// </summary>
[Route("api/[controller]")]
public class ConsultasController : ApiControllerBase
{
    private readonly IConsultaServico _consultaServico;

    public ConsultasController(IConsultaServico consultaServico)
    {
        _consultaServico = consultaServico;
    }

    /// <summary>
    /// Lista consultas com paginação, filtros (petId, veterinario, dataInicio, dataFim, comRetornoPendente)
    /// e ordenação (dataConsulta, custo, dataRetorno).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginaComLinks<ConsultaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginaComLinks<ConsultaResponse>>> Listar([FromQuery] FiltroConsulta filtro)
    {
        var resultado = await _consultaServico.ListarAsync(filtro);
        return Ok(CriarPagina(resultado, LinksDaConsulta));
    }

    /// <summary>Busca uma consulta pelo ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RecursoComLinks<ConsultaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecursoComLinks<ConsultaResponse>>> ObterPorId(int id)
    {
        var consulta = await _consultaServico.ObterPorIdAsync(id);
        return Ok(ComLinks(consulta, LinksDaConsulta(consulta)));
    }

    /// <summary>Registra uma consulta para um pet existente.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(RecursoComLinks<ConsultaResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecursoComLinks<ConsultaResponse>>> Criar([FromBody] CriarConsultaRequest request)
    {
        var consulta = await _consultaServico.CriarAsync(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = consulta.Id }, ComLinks(consulta, LinksDaConsulta(consulta)));
    }

    /// <summary>Atualiza os dados de uma consulta.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarConsultaRequest request)
    {
        await _consultaServico.AtualizarAsync(id, request);
        return NoContent();
    }

    /// <summary>Remove o registro de uma consulta.</summary>
    /// <remarks>Somente o perfil Admin.</remarks>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = PerfisUsuario.Admin)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id)
    {
        await _consultaServico.RemoverAsync(id);
        return NoContent();
    }

    private IEnumerable<Link> LinksDaConsulta(ConsultaResponse consulta) => new[]
    {
        CriarLink(nameof(ObterPorId), "self", MetodoGet, new { id = consulta.Id }),
        CriarLink(nameof(Atualizar), "atualizar", MetodoPut, new { id = consulta.Id }),
        CriarLink(nameof(Remover), "remover", MetodoDelete, new { id = consulta.Id }),
        CriarLink(nameof(PetsController.ObterPorId), "pet", MetodoGet, new { id = consulta.PetId }, NomesControllers.Pets)
    };
}
