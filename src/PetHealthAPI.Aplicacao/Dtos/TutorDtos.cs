namespace PetHealthAPI.Aplicacao.Dtos;

/// <summary>Dados para cadastrar ou atualizar um tutor.</summary>
public record TutorRequest(string Nome, string Email, string Telefone, string? Endereco);

public record TutorResponse(int Id, string Nome, string Email, string Telefone, string? Endereco, DateTime DataCadastro);
