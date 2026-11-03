using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Filtros;

namespace PetHealthAPI.Dominio.Interfaces;

public interface ITutorRepositorio : IRepositorio<Tutor, int, FiltroTutor>
{
    Task<bool> ExisteAsync(int tutorId);

    /// <summary>Verifica se o email já pertence a outro tutor (ignorarId = tutor sendo editado).</summary>
    Task<bool> EmailEmUsoAsync(string email, int? ignorarId);
}
