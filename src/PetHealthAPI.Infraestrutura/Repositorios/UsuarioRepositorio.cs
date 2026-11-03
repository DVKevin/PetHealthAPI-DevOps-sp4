using Microsoft.EntityFrameworkCore;
using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Interfaces;
using PetHealthAPI.Infraestrutura.Persistencia;

namespace PetHealthAPI.Infraestrutura.Repositorios;

public class UsuarioRepositorio : IUsuarioRepositorio
{
    private readonly AppDbContext _contexto;

    public UsuarioRepositorio(AppDbContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        var emailNormalizado = email.Trim().ToLowerInvariant();
        return await _contexto.Usuarios.FirstOrDefaultAsync(u => u.Email == emailNormalizado);
    }

    public async Task<bool> EmailEmUsoAsync(string email)
    {
        var emailNormalizado = email.Trim().ToLowerInvariant();
        return await _contexto.Usuarios.AnyAsync(u => u.Email == emailNormalizado);
    }

    public async Task AdicionarAsync(Usuario usuario)
    {
        await _contexto.Usuarios.AddAsync(usuario);
        await _contexto.SaveChangesAsync();
    }
}
