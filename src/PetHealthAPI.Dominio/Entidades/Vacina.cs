namespace PetHealthAPI.Dominio.Entidades;

/// <summary>
/// Registro da carteira de vacinação digital de um pet.
/// </summary>
public class Vacina
{
    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public DateTime DataAplicacao { get; private set; }
    public DateTime? DataProximaDose { get; private set; }
    public string? Fabricante { get; private set; }
    public string? Lote { get; private set; }
    public string? Veterinario { get; private set; }
    public string? Observacoes { get; private set; }

    public int PetId { get; private set; }
    public Pet? Pet { get; private set; }

    // Construtor exigido pelo EF Core
    protected Vacina() { }

    public Vacina(string nome, DateTime dataAplicacao, DateTime? dataProximaDose, string? fabricante,
                  string? lote, string? veterinario, string? observacoes, int petId)
    {
        Validar(nome, dataAplicacao, dataProximaDose, petId);

        Nome = nome.Trim();
        DataAplicacao = dataAplicacao;
        DataProximaDose = dataProximaDose;
        Fabricante = fabricante?.Trim();
        Lote = lote?.Trim();
        Veterinario = veterinario?.Trim();
        Observacoes = observacoes?.Trim();
        PetId = petId;
    }

    public void Atualizar(string nome, DateTime dataAplicacao, DateTime? dataProximaDose, string? fabricante,
                          string? lote, string? veterinario, string? observacoes)
    {
        Validar(nome, dataAplicacao, dataProximaDose, PetId);

        Nome = nome.Trim();
        DataAplicacao = dataAplicacao;
        DataProximaDose = dataProximaDose;
        Fabricante = fabricante?.Trim();
        Lote = lote?.Trim();
        Veterinario = veterinario?.Trim();
        Observacoes = observacoes?.Trim();
    }

    /// <summary>
    /// Indica se a próxima dose vence entre hoje e os próximos <paramref name="dias"/> dias.
    /// </summary>
    public bool ProximaDoseVenceEm(int dias, DateTime referencia)
    {
        if (!DataProximaDose.HasValue)
            return false;

        return DataProximaDose.Value.Date >= referencia.Date &&
               DataProximaDose.Value.Date <= referencia.Date.AddDays(dias);
    }

    private static void Validar(string nome, DateTime dataAplicacao, DateTime? dataProximaDose, int petId)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome da vacina é obrigatório.", nameof(nome));

        if (dataAplicacao == default)
            throw new ArgumentException("A data de aplicação da vacina é obrigatória.", nameof(dataAplicacao));

        if (dataProximaDose.HasValue && dataProximaDose.Value < dataAplicacao)
            throw new ArgumentException("A data da próxima dose não pode ser anterior à data de aplicação.", nameof(dataProximaDose));

        if (petId <= 0)
            throw new ArgumentException("O pet da vacina deve ser informado.", nameof(petId));
    }
}
