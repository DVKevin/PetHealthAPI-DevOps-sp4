using Microsoft.AspNetCore.Identity;
using PetHealthAPI.Aplicacao.Interfaces;
using PetHealthAPI.Dominio.Entidades;

namespace PetHealthAPI.Infraestrutura.Seguranca;

/// <summary>
/// Hash de senha com o PasswordHasher do ASP.NET Core Identity (PBKDF2 com salt aleatório).
/// </summary>
public class SenhaHasher : ISenhaHasher
{
    private readonly PasswordHasher<Usuario> _hasher = new();

    public string GerarHash(string senha)
    {
        return _hasher.HashPassword(null!, senha);
    }

    public bool Verificar(string senhaHash, string senha)
    {
        var resultado = _hasher.VerifyHashedPassword(null!, senhaHash, senha);
        return resultado != PasswordVerificationResult.Failed;
    }
}
