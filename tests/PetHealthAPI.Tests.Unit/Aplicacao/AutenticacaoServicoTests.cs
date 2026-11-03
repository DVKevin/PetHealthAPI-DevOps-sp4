using FluentAssertions;
using Moq;
using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Aplicacao.Excecoes;
using PetHealthAPI.Aplicacao.Interfaces;
using PetHealthAPI.Aplicacao.Servicos;
using PetHealthAPI.Dominio.Constantes;
using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Interfaces;
using Xunit;

namespace PetHealthAPI.Tests.Unit.Aplicacao;

public class AutenticacaoServicoTests
{
    private readonly Mock<IUsuarioRepositorio> _usuarioRepositorioMock = new();
    private readonly Mock<ISenhaHasher> _senhaHasherMock = new();
    private readonly Mock<IGeradorToken> _geradorTokenMock = new();
    private readonly AutenticacaoServico _servico;

    public AutenticacaoServicoTests()
    {
        _senhaHasherMock.Setup(h => h.GerarHash(It.IsAny<string>())).Returns("hash-da-senha");
        _geradorTokenMock.Setup(g => g.Gerar(It.IsAny<Usuario>()))
            .Returns(new TokenGerado("token-jwt", DateTime.UtcNow.AddHours(1)));

        _servico = new AutenticacaoServico(_usuarioRepositorioMock.Object, _senhaHasherMock.Object,
                                           _geradorTokenMock.Object);
    }

    [Fact]
    public async Task RegistrarAsync_DadosValidos_CriaUsuarioComPerfilUsuario()
    {
        // Arrange
        var request = new RegistrarUsuarioRequest("Ana", "ana@teste.com", "Senha@123");
        _usuarioRepositorioMock.Setup(r => r.EmailEmUsoAsync("ana@teste.com")).ReturnsAsync(false);

        // Act
        var resultado = await _servico.RegistrarAsync(request);

        // Assert
        resultado.Perfil.Should().Be(PerfisUsuario.Usuario);
        _usuarioRepositorioMock.Verify(r => r.AdicionarAsync(It.Is<Usuario>(u => u.SenhaHash == "hash-da-senha")), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345")]
    public async Task RegistrarAsync_SenhaCurta_LancaArgumentException(string senhaInvalida)
    {
        // Arrange
        var request = new RegistrarUsuarioRequest("Ana", "ana@teste.com", senhaInvalida);

        // Act
        var acao = async () => await _servico.RegistrarAsync(request);

        // Assert
        await acao.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*A senha deve ter no mínimo 6 caracteres.*");
        _usuarioRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarAsync_EmailEmUso_LancaConflitoException()
    {
        // Arrange
        var request = new RegistrarUsuarioRequest("Ana", "ana@teste.com", "Senha@123");
        _usuarioRepositorioMock.Setup(r => r.EmailEmUsoAsync("ana@teste.com")).ReturnsAsync(true);

        // Act
        var acao = async () => await _servico.RegistrarAsync(request);

        // Assert
        await acao.Should().ThrowAsync<ConflitoException>()
            .WithMessage(AutenticacaoServico.MensagemEmailDuplicado);
    }

    [Fact]
    public async Task LoginAsync_CredenciaisValidas_RetornaTokenBearer()
    {
        // Arrange
        var usuario = new Usuario("Ana", "ana@teste.com", "hash-da-senha", PerfisUsuario.Usuario);
        _usuarioRepositorioMock.Setup(r => r.ObterPorEmailAsync("ana@teste.com")).ReturnsAsync(usuario);
        _senhaHasherMock.Setup(h => h.Verificar("hash-da-senha", "Senha@123")).Returns(true);

        // Act
        var resultado = await _servico.LoginAsync(new LoginRequest("ANA@teste.com", "Senha@123"));

        // Assert
        resultado.Token.Should().Be("token-jwt");
        resultado.Tipo.Should().Be("Bearer");
        _geradorTokenMock.Verify(g => g.Gerar(usuario), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_SenhaIncorreta_LancaCredenciaisInvalidasException()
    {
        // Arrange
        var usuario = new Usuario("Ana", "ana@teste.com", "hash-da-senha", PerfisUsuario.Usuario);
        _usuarioRepositorioMock.Setup(r => r.ObterPorEmailAsync("ana@teste.com")).ReturnsAsync(usuario);
        _senhaHasherMock.Setup(h => h.Verificar("hash-da-senha", "errada")).Returns(false);

        // Act
        var acao = async () => await _servico.LoginAsync(new LoginRequest("ana@teste.com", "errada"));

        // Assert
        await acao.Should().ThrowAsync<CredenciaisInvalidasException>()
            .WithMessage(CredenciaisInvalidasException.MensagemPadrao);
        _geradorTokenMock.Verify(g => g.Gerar(It.IsAny<Usuario>()), Times.Never);
    }
}
