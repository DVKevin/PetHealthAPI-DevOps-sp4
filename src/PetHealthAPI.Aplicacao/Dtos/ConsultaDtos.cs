namespace PetHealthAPI.Aplicacao.Dtos;

/// <summary>Dados para registrar uma consulta.</summary>
public record CriarConsultaRequest(
    DateTime DataConsulta,
    string Motivo,
    string? Veterinario,
    string? Clinica,
    string? Diagnostico,
    string? Tratamento,
    decimal? Custo,
    DateTime? DataRetorno,
    string? Observacoes,
    int PetId);

/// <summary>Dados para atualizar uma consulta (o pet não pode ser trocado).</summary>
public record AtualizarConsultaRequest(
    DateTime DataConsulta,
    string Motivo,
    string? Veterinario,
    string? Clinica,
    string? Diagnostico,
    string? Tratamento,
    decimal? Custo,
    DateTime? DataRetorno,
    string? Observacoes);

public record ConsultaResponse(
    int Id,
    DateTime DataConsulta,
    string Motivo,
    string? Veterinario,
    string? Clinica,
    string? Diagnostico,
    string? Tratamento,
    decimal? Custo,
    DateTime? DataRetorno,
    string? Observacoes,
    int PetId,
    bool RetornoPendente);
