using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Filtros;

namespace PetHealthAPI.Dominio.Interfaces;

/// <summary>
/// Repositório NoSQL (MongoDB) do diário de cuidados. O Id é o ObjectId em formato string.
/// </summary>
public interface IRegistroDiarioRepositorio : IRepositorio<RegistroDiario, string, FiltroRegistroDiario>
{
    Task<int> SomarPontosDoPetAsync(int petId);
}
