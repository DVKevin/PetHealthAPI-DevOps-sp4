using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Filtros;

namespace PetHealthAPI.Dominio.Interfaces;

public interface IPetRepositorio : IRepositorio<Pet, int, FiltroPet>
{
    Task<bool> ExisteAsync(int petId);
}
