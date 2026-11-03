namespace PetHealthAPI.Aplicacao.Dtos;

public record RegistrarUsuarioRequest(string Nome, string Email, string Senha);

public record LoginRequest(string Email, string Senha);

public record UsuarioResponse(int Id, string Nome, string Email, string Perfil);

public record TokenResponse(string Token, string Tipo, DateTime ExpiraEm, string Nome, string Email, string Perfil);
