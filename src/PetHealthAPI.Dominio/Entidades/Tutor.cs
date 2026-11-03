using PetHealthAPI.Dominio.Comum;

namespace PetHealthAPI.Dominio.Entidades;

/// <summary>
/// Tutor (dono) de um ou mais pets. Entidade rica: só nasce e só muda em estado válido.
/// </summary>
public class Tutor
{
    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Telefone { get; private set; } = string.Empty;
    public string? Endereco { get; private set; }
    public DateTime DataCadastro { get; private set; }

    public ICollection<Pet> Pets { get; private set; } = new List<Pet>();

    // Construtor exigido pelo EF Core
    protected Tutor() { }

    public Tutor(string nome, string email, string telefone, string? endereco)
    {
        Validar(nome, email, telefone, endereco);

        Nome = nome.Trim();
        Email = email.Trim().ToLowerInvariant();
        Telefone = telefone.Trim();
        Endereco = endereco?.Trim();
        DataCadastro = DateTime.Now;
    }

    public void Atualizar(string nome, string email, string telefone, string? endereco)
    {
        Validar(nome, email, telefone, endereco);

        Nome = nome.Trim();
        Email = email.Trim().ToLowerInvariant();
        Telefone = telefone.Trim();
        Endereco = endereco?.Trim();
    }

    private static void Validar(string nome, string email, string telefone, string? endereco)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome do tutor é obrigatório.", nameof(nome));

        if (nome.Trim().Length > 100)
            throw new ArgumentException("O nome do tutor deve ter no máximo 100 caracteres.", nameof(nome));

        var emailNormalizado = email?.Trim();
        if (!ValidadorEmail.EhValido(emailNormalizado) || emailNormalizado.Length > 150)
            throw new ArgumentException("O email do tutor é inválido.", nameof(email));

        if (string.IsNullOrWhiteSpace(telefone) || telefone.Trim().Length > 20)
            throw new ArgumentException("O telefone do tutor é obrigatório e deve ter no máximo 20 caracteres.", nameof(telefone));

        if (endereco is not null && endereco.Trim().Length > 200)
            throw new ArgumentException("O endereço do tutor deve ter no máximo 200 caracteres.", nameof(endereco));
    }
}
