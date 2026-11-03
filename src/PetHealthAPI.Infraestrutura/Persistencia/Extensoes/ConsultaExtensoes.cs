using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PetHealthAPI.Dominio.Comum;

namespace PetHealthAPI.Infraestrutura.Persistencia.Extensoes;

/// <summary>
/// Paginação e ordenação reutilizadas por todos os repositórios EF Core.
/// </summary>
public static class ConsultaExtensoes
{
    public static IQueryable<T> Ordenar<T, TChave>(this IQueryable<T> consulta,
        Expression<Func<T, TChave>> chave, bool descendente)
    {
        return descendente ? consulta.OrderByDescending(chave) : consulta.OrderBy(chave);
    }

    public static async Task<ResultadoPaginado<T>> PaginarAsync<T>(this IQueryable<T> consulta,
        ParametrosConsulta parametros)
    {
        var totalItens = await consulta.CountAsync();

        var itens = await consulta
            .Skip((parametros.Pagina - 1) * parametros.TamanhoPagina)
            .Take(parametros.TamanhoPagina)
            .ToListAsync();

        return new ResultadoPaginado<T>(itens, parametros.Pagina, parametros.TamanhoPagina, totalItens);
    }
}
