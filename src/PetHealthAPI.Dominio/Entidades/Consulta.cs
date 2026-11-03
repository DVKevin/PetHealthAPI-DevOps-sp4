namespace PetHealthAPI.Dominio.Entidades;

/// <summary>
/// Consulta veterinária que compõe o histórico clínico do pet.
/// </summary>
public class Consulta
{
    public int Id { get; private set; }
    public DateTime DataConsulta { get; private set; }
    public string Motivo { get; private set; } = string.Empty;
    public string? Veterinario { get; private set; }
    public string? Clinica { get; private set; }
    public string? Diagnostico { get; private set; }
    public string? Tratamento { get; private set; }
    public decimal? Custo { get; private set; }
    public DateTime? DataRetorno { get; private set; }
    public string? Observacoes { get; private set; }

    public int PetId { get; private set; }
    public Pet? Pet { get; private set; }

    // Construtor exigido pelo EF Core
    protected Consulta() { }

    public Consulta(DateTime dataConsulta, string motivo, string? veterinario, string? clinica,
                    string? diagnostico, string? tratamento, decimal? custo, DateTime? dataRetorno,
                    string? observacoes, int petId)
    {
        Validar(dataConsulta, motivo, custo, dataRetorno, petId);

        DataConsulta = dataConsulta;
        Motivo = motivo.Trim();
        Veterinario = veterinario?.Trim();
        Clinica = clinica?.Trim();
        Diagnostico = diagnostico?.Trim();
        Tratamento = tratamento?.Trim();
        Custo = custo;
        DataRetorno = dataRetorno;
        Observacoes = observacoes?.Trim();
        PetId = petId;
    }

    public void Atualizar(DateTime dataConsulta, string motivo, string? veterinario, string? clinica,
                          string? diagnostico, string? tratamento, decimal? custo, DateTime? dataRetorno,
                          string? observacoes)
    {
        Validar(dataConsulta, motivo, custo, dataRetorno, PetId);

        DataConsulta = dataConsulta;
        Motivo = motivo.Trim();
        Veterinario = veterinario?.Trim();
        Clinica = clinica?.Trim();
        Diagnostico = diagnostico?.Trim();
        Tratamento = tratamento?.Trim();
        Custo = custo;
        DataRetorno = dataRetorno;
        Observacoes = observacoes?.Trim();
    }

    /// <summary>Indica se existe retorno agendado a partir da data de referência.</summary>
    public bool TemRetornoPendente(DateTime referencia) =>
        DataRetorno.HasValue && DataRetorno.Value.Date >= referencia.Date;

    private static void Validar(DateTime dataConsulta, string motivo, decimal? custo,
                                DateTime? dataRetorno, int petId)
    {
        if (dataConsulta == default)
            throw new ArgumentException("A data da consulta é obrigatória.", nameof(dataConsulta));

        if (string.IsNullOrWhiteSpace(motivo))
            throw new ArgumentException("O motivo da consulta é obrigatório.", nameof(motivo));

        if (custo < 0)
            throw new ArgumentException("O custo da consulta não pode ser negativo.", nameof(custo));

        if (dataRetorno.HasValue && dataRetorno.Value < dataConsulta)
            throw new ArgumentException("A data de retorno não pode ser anterior à data da consulta.", nameof(dataRetorno));

        if (petId <= 0)
            throw new ArgumentException("O pet da consulta deve ser informado.", nameof(petId));
    }
}
