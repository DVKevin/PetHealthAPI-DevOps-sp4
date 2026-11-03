using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Filtros;
using PetHealthAPI.Dominio.Interfaces;
using PetHealthAPI.Infraestrutura.Persistencia;
using PetHealthAPI.Infraestrutura.Persistencia.Extensoes;

namespace PetHealthAPI.Infraestrutura.Repositorios;

public class MedicamentoRepositorio : RepositorioBase<Medicamento, FiltroMedicamento>, IMedicamentoRepositorio
{
    public MedicamentoRepositorio(AppDbContext contexto) : base(contexto) { }

    protected override IQueryable<Medicamento> AplicarFiltros(IQueryable<Medicamento> consulta, FiltroMedicamento filtro)
    {
        if (filtro.PetId.HasValue)
            consulta = consulta.Where(m => m.PetId == filtro.PetId.Value);

        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            var nome = filtro.Nome.Trim().ToLowerInvariant();
            consulta = consulta.Where(m => m.Nome.ToLower().Contains(nome));
        }

        if (filtro.Ativo.HasValue)
            consulta = consulta.Where(m => m.Ativo == filtro.Ativo.Value);

        return consulta;
    }

    protected override IQueryable<Medicamento> AplicarOrdenacao(IQueryable<Medicamento> consulta, FiltroMedicamento filtro)
    {
        var descendente = filtro.EhDescendente();

        return filtro.OrdenarPor?.Trim().ToLower() switch
        {
            "nome" => consulta.Ordenar(m => m.Nome, descendente),
            "datainicio" => consulta.Ordenar(m => m.DataInicio, descendente),
            "datafim" => consulta.Ordenar(m => m.DataFim, descendente),
            _ => consulta.Ordenar(m => m.Id, descendente)
        };
    }
}
