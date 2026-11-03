namespace PetHealthAPI.Aplicacao.Dtos;

/// <summary>Check-in diário de cuidados do pet (MongoDB).</summary>
public record CriarRegistroDiarioRequest(
    int PetId,
    DateTime Data,
    bool Alimentacao,
    bool Hidratacao,
    bool Passeio,
    bool MedicacaoEmDia,
    string? Humor,
    string? Observacoes);

public record AtualizarRegistroDiarioRequest(
    DateTime Data,
    bool Alimentacao,
    bool Hidratacao,
    bool Passeio,
    bool MedicacaoEmDia,
    string? Humor,
    string? Observacoes);

public record RegistroDiarioResponse(
    string Id,
    int PetId,
    DateTime Data,
    bool Alimentacao,
    bool Hidratacao,
    bool Passeio,
    bool MedicacaoEmDia,
    string? Humor,
    string? Observacoes,
    int Pontos,
    DateTime DataRegistro);

/// <summary>Gamificação: total de XP e nível do pet (a cada 100 pontos, sobe um nível).</summary>
public record ResumoPontosResponse(int PetId, int TotalPontos, int Nivel, int PontosParaProximoNivel);
