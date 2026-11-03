using Microsoft.EntityFrameworkCore;
using PetHealthAPI.Dominio.Comum;
using PetHealthAPI.Dominio.Interfaces;
using PetHealthAPI.Infraestrutura.Persistencia;
using PetHealthAPI.Infraestrutura.Persistencia.Extensoes;

namespace PetHealthAPI.Infraestrutura.Repositorios;

/// <summary>
/// Implementação base do padrão Repository com EF Core.
/// Cada repositório concreto define apenas seus filtros e sua ordenação (Template Method).
/// </summary>
public abstract class RepositorioBase<TEntidade, TFiltro> : IRepositorio<TEntidade, int, TFiltro>
    where TEntidade : class
    where TFiltro : ParametrosConsulta
{
    protected readonly AppDbContext Contexto;

    protected RepositorioBase(AppDbContext contexto)
    {
        Contexto = contexto;
    }

    public virtual async Task<TEntidade?> ObterPorIdAsync(int id)
    {
        return await Contexto.Set<TEntidade>().FindAsync(id);
    }

    public async Task<ResultadoPaginado<TEntidade>> ListarAsync(TFiltro filtro)
    {
        var consulta = Contexto.Set<TEntidade>().AsNoTracking();
        consulta = AplicarFiltros(consulta, filtro);
        consulta = AplicarOrdenacao(consulta, filtro);

        return await consulta.PaginarAsync(filtro);
    }

    public async Task AdicionarAsync(TEntidade entidade)
    {
        await Contexto.Set<TEntidade>().AddAsync(entidade);
        await Contexto.SaveChangesAsync();
    }

    public async Task AtualizarAsync(TEntidade entidade)
    {
        if (Contexto.Entry(entidade).State == EntityState.Detached)
            Contexto.Set<TEntidade>().Update(entidade);

        await Contexto.SaveChangesAsync();
    }

    public async Task RemoverAsync(TEntidade entidade)
    {
        Contexto.Set<TEntidade>().Remove(entidade);
        await Contexto.SaveChangesAsync();
    }

    protected abstract IQueryable<TEntidade> AplicarFiltros(IQueryable<TEntidade> consulta, TFiltro filtro);

    protected abstract IQueryable<TEntidade> AplicarOrdenacao(IQueryable<TEntidade> consulta, TFiltro filtro);
}
