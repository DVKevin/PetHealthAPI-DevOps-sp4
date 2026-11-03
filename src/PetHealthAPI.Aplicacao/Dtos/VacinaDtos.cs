namespace PetHealthAPI.Aplicacao.Dtos;

/// <summary>Dados para registrar uma vacina.</summary>
public record CriarVacinaRequest(
    string Nome,
    DateTime DataAplicacao,
    DateTime? DataProximaDose,
    string? Fabricante,
    string? Lote,
    string? Veterinario,
    string? Observacoes,
    int PetId);

/// <summary>Dados para atualizar uma vacina (o pet não pode ser trocado).</summary>
public record AtualizarVacinaRequest(
    string Nome,
    DateTime DataAplicacao,
    DateTime? DataProximaDose,
    string? Fabricante,
    string? Lote,
    string? Veterinario,
    string? Observacoes);

public record VacinaResponse(
    int Id,
    string Nome,
    DateTime DataAplicacao,
    DateTime? DataProximaDose,
    string? Fabricante,
    string? Lote,
    string? Veterinario,
    string? Observacoes,
    int PetId,
    bool ProximaDoseEm30Dias);
