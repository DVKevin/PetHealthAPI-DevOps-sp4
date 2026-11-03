using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Aplicacao.Interfaces;
using PetHealthAPI.Dominio.Constantes;
using PetHealthAPI.Dominio.Filtros;
using PetHealthAPI.Hateoas;

namespace PetHealthAPI.Controllers;

/// <summary>
/// Carteira de vacinação digital dos pets.
/// </summary>
[Route("api/[controller]")]
public class VacinasController : ApiControllerBase
{
    private readonly IVacinaServico _vacinaServico;

    public VacinasController(IVacinaServico vacinaServico)
    {
        _vacinaServico = vacinaServico;
    }

    /// <summary>
    /// Lista vacinas com paginação, filtros (petId, nome, vencendoEmDias)
    /// e ordenação (nome, dataAplicacao, dataProximaDose).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginaComLinks<VacinaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginaComLinks<VacinaResponse>>> Listar([FromQuery] FiltroVacina filtro)
    {
        var resultado = await _vacinaServico.ListarAsync(filtro);
        return Ok(CriarPagina(resultado, LinksDaVacina));
    }

    /// <summary>Busca uma vacina pelo ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RecursoComLinks<VacinaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecursoComLinks<VacinaResponse>>> ObterPorId(int id)
    {
        var vacina = await _vacinaServico.ObterPorIdAsync(id);
        return Ok(ComLinks(vacina, LinksDaVacina(vacina)));
    }

    /// <summary>Registra uma vacina para um pet existente.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(RecursoComLinks<VacinaResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecursoComLinks<VacinaResponse>>> Criar([FromBody] CriarVacinaRequest request)
    {
        var vacina = await _vacinaServico.CriarAsync(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = vacina.Id }, ComLinks(vacina, LinksDaVacina(vacina)));
    }

    /// <summary>Atualiza os dados de uma vacina.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarVacinaRequest request)
    {
        await _vacinaServico.AtualizarAsync(id, request);
        return NoContent();
    }

    /// <summary>Remove o registro de uma vacina.</summary>
    /// <remarks>Somente o perfil Admin.</remarks>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = PerfisUsuario.Admin)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id)
    {
        await _vacinaServico.RemoverAsync(id);
        return NoContent();
    }

    private IEnumerable<Link> LinksDaVacina(VacinaResponse vacina) => new[]
    {
        CriarLink(nameof(ObterPorId), "self", MetodoGet, new { id = vacina.Id }),
        CriarLink(nameof(Atualizar), "atualizar", MetodoPut, new { id = vacina.Id }),
        CriarLink(nameof(Remover), "remover", MetodoDelete, new { id = vacina.Id }),
        CriarLink(nameof(PetsController.ObterPorId), "pet", MetodoGet, new { id = vacina.PetId }, NomesControllers.Pets)
    };
}
