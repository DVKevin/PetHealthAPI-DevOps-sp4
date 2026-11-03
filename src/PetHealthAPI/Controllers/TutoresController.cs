using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Aplicacao.Interfaces;
using PetHealthAPI.Dominio.Constantes;
using PetHealthAPI.Dominio.Filtros;
using PetHealthAPI.Hateoas;

namespace PetHealthAPI.Controllers;

/// <summary>
/// Gerenciamento de tutores (donos de pets).
/// </summary>
[Route("api/[controller]")]
public class TutoresController : ApiControllerBase
{
    private readonly ITutorServico _tutorServico;
    private readonly IPetServico _petServico;

    public TutoresController(ITutorServico tutorServico, IPetServico petServico)
    {
        _tutorServico = tutorServico;
        _petServico = petServico;
    }

    /// <summary>Lista tutores com paginação, filtros (nome, email) e ordenação (nome, email, dataCadastro).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginaComLinks<TutorResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginaComLinks<TutorResponse>>> Listar([FromQuery] FiltroTutor filtro)
    {
        var resultado = await _tutorServico.ListarAsync(filtro);
        return Ok(CriarPagina(resultado, tutor => LinksDoTutor(tutor.Id)));
    }

    /// <summary>Busca um tutor pelo ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RecursoComLinks<TutorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecursoComLinks<TutorResponse>>> ObterPorId(int id)
    {
        var tutor = await _tutorServico.ObterPorIdAsync(id);
        return Ok(ComLinks(tutor, LinksDoTutor(id)));
    }

    /// <summary>Lista os pets de um tutor (paginado).</summary>
    [HttpGet("{id:int}/pets")]
    [ProducesResponseType(typeof(PaginaComLinks<PetResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaginaComLinks<PetResponse>>> ListarPets(int id, [FromQuery] FiltroPet filtro)
    {
        await _tutorServico.ObterPorIdAsync(id); // 404 se o tutor não existir

        filtro.TutorId = id;
        var resultado = await _petServico.ListarAsync(filtro);

        return Ok(CriarPagina(resultado, pet => new[]
        {
            CriarLink(nameof(PetsController.ObterPorId), "self", MetodoGet, new { id = pet.Id }, NomesControllers.Pets)
        }));
    }

    /// <summary>Cadastra um novo tutor.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(RecursoComLinks<TutorResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RecursoComLinks<TutorResponse>>> Criar([FromBody] TutorRequest request)
    {
        var tutor = await _tutorServico.CriarAsync(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = tutor.Id }, ComLinks(tutor, LinksDoTutor(tutor.Id)));
    }

    /// <summary>Atualiza os dados de um tutor.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] TutorRequest request)
    {
        await _tutorServico.AtualizarAsync(id, request);
        return NoContent();
    }

    /// <summary>Remove um tutor (e, em cascata, seus pets e históricos).</summary>
    /// <remarks>Somente o perfil Admin.</remarks>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = PerfisUsuario.Admin)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id)
    {
        await _tutorServico.RemoverAsync(id);
        return NoContent();
    }

    private IEnumerable<Link> LinksDoTutor(int id) => new[]
    {
        CriarLink(nameof(ObterPorId), "self", MetodoGet, new { id }),
        CriarLink(nameof(Atualizar), "atualizar", MetodoPut, new { id }),
        CriarLink(nameof(Remover), "remover", MetodoDelete, new { id }),
        CriarLink(nameof(ListarPets), "pets", MetodoGet, new { id })
    };
}
