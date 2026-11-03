using System.Diagnostics.CodeAnalysis;
namespace PetHealthAPI.Dominio.Comum;

/// <summary>
/// Regra de formato de email compartilhada entre as entidades (evita duplicação — DRY).
/// </summary>
public static class ValidadorEmail
{
    public static bool EhValido([NotNullWhen(true)] string? email)
    {
        if (string.IsNullOrWhiteSpace(email) || email.Contains(' '))
            return false;

        var posicaoArroba = email.IndexOf('@');
        if (posicaoArroba <= 0 || posicaoArroba != email.LastIndexOf('@'))
            return false;

        var posicaoPonto = email.IndexOf('.', posicaoArroba);
        return posicaoPonto > posicaoArroba + 1 && posicaoPonto < email.Length - 1;
    }
}
