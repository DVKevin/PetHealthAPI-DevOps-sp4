using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using PetHealthAPI.Dominio.Comum;
using PetHealthAPI.Hateoas;

namespace PetHealthAPI.Controllers;

/// <summary>
/// Base dos controllers: concentra a montagem de links HATEOAS e de páginas,
/// para que cada controller só descreva os links do seu próprio recurso.
/// Todos os endpoints exigem token JWT, exceto os marcados com [AllowAnonymous].
/// </summary>
[ApiController]
[Authorize]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
public abstract class ApiControllerBase : ControllerBase
{
    protected const string MetodoGet = "GET";
    protected const string MetodoPost = "POST";
    protected const string MetodoPut = "PUT";
    protected const string MetodoPatch = "PATCH";
    protected const string MetodoDelete = "DELETE";

    protected Link CriarLink(string acao, string rel, string metodo, object? valores = null, string? controlador = null)
    {
        var href = Url.Action(acao, controlador, valores, Request.Scheme) ?? string.Empty;
        return new Link(href, rel, metodo);
    }

    protected static RecursoComLinks<T> ComLinks<T>(T dados, IEnumerable<Link> links)
    {
        return new RecursoComLinks<T>(dados, links.ToList());
    }

    protected PaginaComLinks<T> CriarPagina<T>(ResultadoPaginado<T> resultado, Func<T, IEnumerable<Link>> linksDoItem)
    {
        var itens = resultado.Itens
            .Select(item => ComLinks(item, linksDoItem(item)))
            .ToList();

        var links = new List<Link>
        {
            new(UrlDaPagina(resultado.Pagina, resultado.TamanhoPagina), "self", MetodoGet)
        };

        if (resultado.TemPaginaAnterior)
        {
            links.Add(new Link(UrlDaPagina(1, resultado.TamanhoPagina), "primeira", MetodoGet));
            links.Add(new Link(UrlDaPagina(resultado.Pagina - 1, resultado.TamanhoPagina), "anterior", MetodoGet));
        }

        if (resultado.TemProximaPagina)
        {
            links.Add(new Link(UrlDaPagina(resultado.Pagina + 1, resultado.TamanhoPagina), "proxima", MetodoGet));
            links.Add(new Link(UrlDaPagina(resultado.TotalPaginas, resultado.TamanhoPagina), "ultima", MetodoGet));
        }

        return new PaginaComLinks<T>(itens, resultado.Pagina, resultado.TamanhoPagina,
                                     resultado.TotalItens, resultado.TotalPaginas, links);
    }

    /// <summary>Monta a URL de uma página mantendo os filtros e a ordenação da requisição atual.</summary>
    private string UrlDaPagina(int pagina, int tamanhoPagina)
    {
        var parametros = QueryHelpers.ParseQuery(Request.QueryString.Value)
            .Where(p => !p.Key.Equals("pagina", StringComparison.OrdinalIgnoreCase) &&
                        !p.Key.Equals("tamanhoPagina", StringComparison.OrdinalIgnoreCase))
            .Select(p => new KeyValuePair<string, string?>(p.Key, p.Value.ToString()))
            .ToList();

        parametros.Add(new KeyValuePair<string, string?>("pagina", pagina.ToString()));
        parametros.Add(new KeyValuePair<string, string?>("tamanhoPagina", tamanhoPagina.ToString()));

        var urlBase = $"{Request.Scheme}://{Request.Host}{Request.PathBase}{Request.Path}";
        return QueryHelpers.AddQueryString(urlBase, parametros);
    }
}
