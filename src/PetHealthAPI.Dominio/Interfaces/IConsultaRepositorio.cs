using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Filtros;

namespace PetHealthAPI.Dominio.Interfaces;

public interface IConsultaRepositorio : IRepositorio<Consulta, int, FiltroConsulta>
{
}
