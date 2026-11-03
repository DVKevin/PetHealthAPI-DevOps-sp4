using PetHealthAPI.Dominio.Comum;
using PetHealthAPI.Dominio.Constantes;

namespace PetHealthAPI.Dominio.Entidades;

/// <summary>
/// Usuário da API (autenticação JWT). Guarda apenas o hash da senha, nunca a senha.
/// </summary>
public class Usuario
{
    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public string Perfil { get; private set; } = PerfisUsuario.Usuario;
    public DateTime DataCadastro { get; private set; }

    // Construtor exigido pelo EF Core
    protected Usuario() { }

    public Usuario(string nome, string email, string senhaHash, string perfil)
    {
        Validar(nome, email, senhaHash, perfil);

        Nome = nome.Trim();
        Email = email.Trim().ToLowerInvariant();
        SenhaHash = senhaHash;
        Perfil = perfil;
        DataCadastro = DateTime.Now;
    }

    public bool EhAdmin() => Perfil == PerfisUsuario.Admin;

    private static void Validar(string nome, string email, string senhaHash, string perfil)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome do usuário é obrigatório.", nameof(nome));

        if (!ValidadorEmail.EhValido(email?.Trim()))
            throw new ArgumentException("O email do usuário é inválido.", nameof(email));

        if (string.IsNullOrWhiteSpace(senhaHash))
            throw new ArgumentException("A senha do usuário é obrigatória.", nameof(senhaHash));

        if (!PerfisUsuario.Todos.Contains(perfil))
            throw new ArgumentException("O perfil do usuário é inválido.", nameof(perfil));
    }
}
