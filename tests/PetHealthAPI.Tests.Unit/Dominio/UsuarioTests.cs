using FluentAssertions;
using PetHealthAPI.Dominio.Constantes;
using PetHealthAPI.Dominio.Entidades;
using Xunit;

namespace PetHealthAPI.Tests.Unit.Dominio;

public class UsuarioTests
{
    [Fact]
    public void Construtor_PerfilAdmin_EhAdmin()
    {
        // Arrange & Act
        var usuario = new Usuario("Admin", "ADMIN@pethealth.com", "hash", PerfisUsuario.Admin);

        // Assert
        usuario.EhAdmin().Should().BeTrue();
        usuario.Email.Should().Be("admin@pethealth.com");
    }

    [Fact]
    public void Construtor_PerfilInexistente_LancaArgumentException()
    {
        // Arrange
        Action acao = () => new Usuario("Ana", "ana@email.com", "hash", "SuperUsuario");

        // Act & Assert
        acao.Should().Throw<ArgumentException>().WithMessage("*O perfil do usuário é inválido.*");
    }

    [Fact]
    public void Construtor_SemHashDeSenha_LancaArgumentException()
    {
        // Arrange
        Action acao = () => new Usuario("Ana", "ana@email.com", "", PerfisUsuario.Usuario);

        // Act & Assert
        acao.Should().Throw<ArgumentException>().WithMessage("*A senha do usuário é obrigatória.*");
    }
}
