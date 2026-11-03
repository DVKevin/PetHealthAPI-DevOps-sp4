namespace PetHealthAPI.Dominio.Entidades;

/// <summary>
/// Medicamento prescrito para um pet (tratamento ativo ou encerrado).
/// </summary>
public class Medicamento
{
    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Dosagem { get; private set; } = string.Empty;
    public string Frequencia { get; private set; } = string.Empty;
    public DateTime DataInicio { get; private set; }
    public DateTime? DataFim { get; private set; }
    public string? VeterinarioPrescreveu { get; private set; }
    public bool Ativo { get; private set; }
    public string? Observacoes { get; private set; }

    public int PetId { get; private set; }
    public Pet? Pet { get; private set; }

    // Construtor exigido pelo EF Core
    protected Medicamento() { }

    public Medicamento(string nome, string dosagem, string frequencia, DateTime dataInicio, DateTime? dataFim,
                       string? veterinarioPrescreveu, string? observacoes, int petId)
    {
        Validar(nome, dosagem, frequencia, dataInicio, dataFim, petId);

        Nome = nome.Trim();
        Dosagem = dosagem.Trim();
        Frequencia = frequencia.Trim();
        DataInicio = dataInicio;
        DataFim = dataFim;
        VeterinarioPrescreveu = veterinarioPrescreveu?.Trim();
        Observacoes = observacoes?.Trim();
        PetId = petId;
        Ativo = true;
    }

    public void Atualizar(string nome, string dosagem, string frequencia, DateTime dataInicio, DateTime? dataFim,
                          string? veterinarioPrescreveu, string? observacoes, bool ativo)
    {
        Validar(nome, dosagem, frequencia, dataInicio, dataFim, PetId);

        Nome = nome.Trim();
        Dosagem = dosagem.Trim();
        Frequencia = frequencia.Trim();
        DataInicio = dataInicio;
        DataFim = dataFim;
        VeterinarioPrescreveu = veterinarioPrescreveu?.Trim();
        Observacoes = observacoes?.Trim();
        Ativo = ativo;
    }

    /// <summary>Encerra o tratamento na data informada.</summary>
    public void Encerrar(DateTime dataFim)
    {
        if (dataFim < DataInicio)
            throw new ArgumentException("A data de fim não pode ser anterior à data de início.", nameof(dataFim));

        DataFim = dataFim;
        Ativo = false;
    }

    private static void Validar(string nome, string dosagem, string frequencia, DateTime dataInicio,
                                DateTime? dataFim, int petId)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome do medicamento é obrigatório.", nameof(nome));

        if (string.IsNullOrWhiteSpace(dosagem))
            throw new ArgumentException("A dosagem do medicamento é obrigatória.", nameof(dosagem));

        if (string.IsNullOrWhiteSpace(frequencia))
            throw new ArgumentException("A frequência do medicamento é obrigatória.", nameof(frequencia));

        if (dataInicio == default)
            throw new ArgumentException("A data de início do medicamento é obrigatória.", nameof(dataInicio));

        if (dataFim.HasValue && dataFim.Value < dataInicio)
            throw new ArgumentException("A data de fim não pode ser anterior à data de início.", nameof(dataFim));

        if (petId <= 0)
            throw new ArgumentException("O pet do medicamento deve ser informado.", nameof(petId));
    }
}
