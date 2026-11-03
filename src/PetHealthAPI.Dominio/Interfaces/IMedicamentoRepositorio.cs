using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Filtros;

namespace PetHealthAPI.Dominio.Interfaces;

public interface IMedicamentoRepositorio : IRepositorio<Medicamento, int, FiltroMedicamento>
{
}
