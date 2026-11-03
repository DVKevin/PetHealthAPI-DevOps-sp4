using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Aplicacao.Excecoes;
using PetHealthAPI.Aplicacao.Interfaces;
using PetHealthAPI.Aplicacao.Mapeamentos;
using PetHealthAPI.Dominio.Constantes;
using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Interfaces;

namespace PetHealthAPI.Aplicacao.Servicos;

/// <summary>
/// Cadastro e login. O cadastro público sempre cria perfil "Usuario";
/// o perfil "Admin" vem do seed do banco.
/// </summary>
public class AutenticacaoServico : IAutenticacaoServico
{
    public const int TamanhoMinimoSenha = 6;
    public const string MensagemEmailDuplicado = "Já existe um usuário cadastrado com este email.";

    private readonly IUsuarioRepositorio _usuarioRepositorio;
    private readonly ISenhaHasher _senhaHasher;
    private readonly IGeradorToken _geradorToken;

    public AutenticacaoServico(IUsuarioRepositorio usuarioRepositorio, ISenhaHasher senhaHasher,
                               IGeradorToken geradorToken)
    {
        _usuarioRepositorio = usuarioRepositorio;
        _senhaHasher = senhaHasher;
        _geradorToken = geradorToken;
    }

    public async Task<UsuarioResponse> RegistrarAsync(RegistrarUsuarioRequest request)
    {
        ValidarSenha(request.Senha);

        var usuario = new Usuario(request.Nome, request.Email, _senhaHasher.GerarHash(request.Senha),
                                  PerfisUsuario.Usuario);

        if (await _usuarioRepositorio.EmailEmUsoAsync(usuario.Email))
            throw new ConflitoException(MensagemEmailDuplicado);

        await _usuarioRepositorio.AdicionarAsync(usuario);
        return usuario.ParaResponse();
    }

    public async Task<TokenResponse> LoginAsync(LoginRequest request)
    {
        var emailNormalizado = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
        var usuario = await _usuarioRepositorio.ObterPorEmailAsync(emailNormalizado);

        if (usuario is null || !_senhaHasher.Verificar(usuario.SenhaHash, request.Senha ?? string.Empty))
            throw new CredenciaisInvalidasException();

        var token = _geradorToken.Gerar(usuario);
        return new TokenResponse(token.Token, "Bearer", token.ExpiraEm, usuario.Nome, usuario.Email, usuario.Perfil);
    }

    private static void ValidarSenha(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha) || senha.Length < TamanhoMinimoSenha)
            throw new ArgumentException("A senha deve ter no mínimo 6 caracteres.", nameof(senha));
    }
}
