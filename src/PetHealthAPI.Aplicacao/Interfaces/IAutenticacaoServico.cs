using PetHealthAPI.Aplicacao.Dtos;

namespace PetHealthAPI.Aplicacao.Interfaces;

public interface IAutenticacaoServico
{
    Task<UsuarioResponse> RegistrarAsync(RegistrarUsuarioRequest request);
    Task<TokenResponse> LoginAsync(LoginRequest request);
}
