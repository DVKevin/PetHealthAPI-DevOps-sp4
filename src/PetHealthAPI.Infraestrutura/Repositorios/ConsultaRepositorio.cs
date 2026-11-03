using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Filtros;
using PetHealthAPI.Dominio.Interfaces;
using PetHealthAPI.Infraestrutura.Persistencia;
using PetHealthAPI.Infraestrutura.Persistencia.Extensoes;

namespace PetHealthAPI.Infraestrutura.Repositorios;

public class ConsultaRepositorio : RepositorioBase<Consulta, FiltroConsulta>, IConsultaRepositorio
{
    public ConsultaRepositorio(AppDbContext contexto) : base(contexto) { }

    protected override IQueryable<Consulta> AplicarFiltros(IQueryable<Consulta> consulta, FiltroConsulta filtro)
    {
        if (filtro.PetId.HasValue)
            consulta = consulta.Where(c => c.PetId == filtro.PetId.Value);

        if (!string.IsNullOrWhiteSpace(filtro.Veterinario))
        {
            var veterinario = filtro.Veterinario.Trim().ToLowerInvariant();
            consulta = consulta.Where(c => c.Veterinario != null && c.Veterinario.ToLower().Contains(veterinario));
        }

        if (filtro.DataInicio.HasValue)
            consulta = consulta.Where(c => c.DataConsulta >= filtro.DataInicio.Value);

        if (filtro.DataFim.HasValue)
            consulta = consulta.Where(c => c.DataConsulta <= filtro.DataFim.Value);

        if (filtro.ComRetornoPendente == true)
        {
            var hoje = DateTime.Today;
            consulta = consulta.Where(c => c.DataRetorno >= hoje);
        }

        return consulta;
    }

    protected override IQueryable<Consulta> AplicarOrdenacao(IQueryable<Consulta> consulta, FiltroConsulta filtro)
    {
        var descendente = filtro.EhDescendente();

        return filtro.OrdenarPor?.Trim().ToLower() switch
        {
            "dataconsulta" => consulta.Ordenar(c => c.DataConsulta, descendente),
            "custo" => consulta.Ordenar(c => c.Custo, descendente),
            "dataretorno" => consulta.Ordenar(c => c.DataRetorno, descendente),
            _ => consulta.Ordenar(c => c.Id, descendente)
        };
    }
}
