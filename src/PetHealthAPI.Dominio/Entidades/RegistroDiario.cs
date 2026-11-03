namespace PetHealthAPI.Dominio.Entidades;

/// <summary>
/// Registro diário de cuidados do pet (check-in gamificado), persistido no MongoDB.
/// Cada cuidado realizado vale 10 pontos de XP.
/// </summary>
public class RegistroDiario
{
    public const int PontosPorCuidado = 10;
    public const int TamanhoMaximoObservacoes = 500;

    public string Id { get; private set; } = string.Empty;
    public int PetId { get; private set; }
    public DateTime Data { get; private set; }
    public bool Alimentacao { get; private set; }
    public bool Hidratacao { get; private set; }
    public bool Passeio { get; private set; }
    public bool MedicacaoEmDia { get; private set; }
    public string? Humor { get; private set; }
    public string? Observacoes { get; private set; }
    public int Pontos { get; private set; }
    public DateTime DataRegistro { get; private set; }

    // Construtor exigido pelo driver do MongoDB
    protected RegistroDiario() { }

    public RegistroDiario(int petId, DateTime data, bool alimentacao, bool hidratacao, bool passeio,
                          bool medicacaoEmDia, string? humor, string? observacoes)
    {
        Validar(petId, data, observacoes);

        PetId = petId;
        Data = NormalizarDia(data);
        Alimentacao = alimentacao;
        Hidratacao = hidratacao;
        Passeio = passeio;
        MedicacaoEmDia = medicacaoEmDia;
        Humor = humor?.Trim();
        Observacoes = observacoes?.Trim();
        Pontos = CalcularPontos(alimentacao, hidratacao, passeio, medicacaoEmDia);
        DataRegistro = DateTime.Now;
    }

    public void Atualizar(DateTime data, bool alimentacao, bool hidratacao, bool passeio,
                          bool medicacaoEmDia, string? humor, string? observacoes)
    {
        Validar(PetId, data, observacoes);

        Data = NormalizarDia(data);
        Alimentacao = alimentacao;
        Hidratacao = hidratacao;
        Passeio = passeio;
        MedicacaoEmDia = medicacaoEmDia;
        Humor = humor?.Trim();
        Observacoes = observacoes?.Trim();
        Pontos = CalcularPontos(alimentacao, hidratacao, passeio, medicacaoEmDia);
    }

    /// <summary>Usado pelo repositório para atribuir o ObjectId gerado pelo MongoDB.</summary>
    public void DefinirId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("O identificador do registro é obrigatório.", nameof(id));

        Id = id;
    }

    /// <summary>
    /// O registro representa um dia do calendário, sem horário nem fuso: guardado como 00:00 UTC
    /// para que o MongoDB (que armazena em UTC) não desloque a data.
    /// </summary>
    public static DateTime NormalizarDia(DateTime data) => DateTime.SpecifyKind(data.Date, DateTimeKind.Utc);

    public static int CalcularPontos(bool alimentacao, bool hidratacao, bool passeio, bool medicacaoEmDia)
    {
        var cuidadosRealizados = new[] { alimentacao, hidratacao, passeio, medicacaoEmDia }.Count(c => c);
        return cuidadosRealizados * PontosPorCuidado;
    }

    private static void Validar(int petId, DateTime data, string? observacoes)
    {
        if (petId <= 0)
            throw new ArgumentException("O pet do registro deve ser informado.", nameof(petId));

        if (data == default)
            throw new ArgumentException("A data do registro é obrigatória.", nameof(data));

        if (data.Date > DateTime.Now.Date)
            throw new ArgumentException("A data do registro não pode ser futura.", nameof(data));

        if (observacoes is not null && observacoes.Trim().Length > TamanhoMaximoObservacoes)
            throw new ArgumentException("As observações devem ter no máximo 500 caracteres.", nameof(observacoes));
    }
}
