namespace PetHealthAPI.Aplicacao.Dtos;

/// <summary>Dados para registrar um medicamento.</summary>
public record CriarMedicamentoRequest(
    string Nome,
    string Dosagem,
    string Frequencia,
    DateTime DataInicio,
    DateTime? DataFim,
    string? VeterinarioPrescreveu,
    string? Observacoes,
    int PetId);

/// <summary>Dados para atualizar um medicamento (o pet não pode ser trocado).</summary>
public record AtualizarMedicamentoRequest(
    string Nome,
    string Dosagem,
    string Frequencia,
    DateTime DataInicio,
    DateTime? DataFim,
    string? VeterinarioPrescreveu,
    string? Observacoes,
    bool Ativo);

/// <summary>Data de encerramento do tratamento (se vazia, usa a data atual).</summary>
public record EncerrarMedicamentoRequest(DateTime? DataFim);

public record MedicamentoResponse(
    int Id,
    string Nome,
    string Dosagem,
    string Frequencia,
    DateTime DataInicio,
    DateTime? DataFim,
    string? VeterinarioPrescreveu,
    bool Ativo,
    string? Observacoes,
    int PetId);
