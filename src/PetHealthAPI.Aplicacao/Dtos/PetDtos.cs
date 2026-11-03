namespace PetHealthAPI.Aplicacao.Dtos;

/// <summary>Dados para cadastrar ou atualizar um pet.</summary>
public record PetRequest(
    string Nome,
    string Especie,
    string? Raca,
    int? Idade,
    decimal? Peso,
    string? Sexo,
    bool Castrado,
    string? Alergias,
    DateTime? DataNascimento,
    int TutorId);

public record PetResponse(
    int Id,
    string Nome,
    string Especie,
    string? Raca,
    int? Idade,
    decimal? Peso,
    string? Sexo,
    bool Castrado,
    string? Alergias,
    DateTime? DataNascimento,
    DateTime DataCadastro,
    int TutorId,
    string? NomeTutor);
