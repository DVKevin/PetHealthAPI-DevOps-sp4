using PetHealthAPI.Dominio.Entidades;

namespace PetHealthAPI.Dominio.Interfaces;

public interface IUsuarioRepositorio
{
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<bool> EmailEmUsoAsync(string email);
    Task AdicionarAsync(Usuario usuario);
}
