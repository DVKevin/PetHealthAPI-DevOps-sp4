using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Filtros;
using PetHealthAPI.Dominio.Interfaces;
using PetHealthAPI.Infraestrutura.Persistencia;
using PetHealthAPI.Infraestrutura.Persistencia.Extensoes;

namespace PetHealthAPI.Infraestrutura.Repositorios;

public class VacinaRepositorio : RepositorioBase<Vacina, FiltroVacina>, IVacinaRepositorio
{
    public VacinaRepositorio(AppDbContext contexto) : base(contexto) { }

    protected override IQueryable<Vacina> AplicarFiltros(IQueryable<Vacina> consulta, FiltroVacina filtro)
    {
        if (filtro.PetId.HasValue)
            consulta = consulta.Where(v => v.PetId == filtro.PetId.Value);

        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            var nome = filtro.Nome.Trim().ToLowerInvariant();
            consulta = consulta.Where(v => v.Nome.ToLower().Contains(nome));
        }

        if (filtro.VencendoEmDias.HasValue)
        {
            var hoje = DateTime.Today;
            var limite = hoje.AddDays(filtro.VencendoEmDias.Value + 1);
            consulta = consulta.Where(v => v.DataProximaDose >= hoje && v.DataProximaDose < limite);
        }

        return consulta;
    }

    protected override IQueryable<Vacina> AplicarOrdenacao(IQueryable<Vacina> consulta, FiltroVacina filtro)
    {
        var descendente = filtro.EhDescendente();

        return filtro.OrdenarPor?.Trim().ToLower() switch
        {
            "nome" => consulta.Ordenar(v => v.Nome, descendente),
            "dataaplicacao" => consulta.Ordenar(v => v.DataAplicacao, descendente),
            "dataproximadose" => consulta.Ordenar(v => v.DataProximaDose, descendente),
            _ => consulta.Ordenar(v => v.Id, descendente)
        };
    }
}
