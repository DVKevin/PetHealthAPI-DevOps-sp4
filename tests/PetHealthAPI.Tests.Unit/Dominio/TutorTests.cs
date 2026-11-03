using FluentAssertions;
using PetHealthAPI.Dominio.Entidades;
using Xunit;

namespace PetHealthAPI.Tests.Unit.Dominio;

public class TutorTests
{
    [Fact]
    public void Construtor_DadosValidos_NormalizaEmailETextos()
    {
        // Arrange & Act
        var tutor = new Tutor("  Ana Souza ", " Ana@Email.COM ", " 11999999999 ", " Rua A ");

        // Assert
        tutor.Nome.Should().Be("Ana Souza");
        tutor.Email.Should().Be("ana@email.com");
        tutor.Telefone.Should().Be("11999999999");
        tutor.Endereco.Should().Be("Rua A");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Construtor_NomeInvalido_LancaArgumentException(string nomeInvalido)
    {
        // Arrange
        Action acao = () => new Tutor(nomeInvalido, "ana@email.com", "11999999999", null);

        // Act & Assert
        acao.Should().Throw<ArgumentException>().WithMessage("*O nome do tutor é obrigatório.*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("sem-arroba.com")]
    [InlineData("ana@dominio")]
    [InlineData("@dominio.com")]
    [InlineData("ana @email.com")]
    public void Construtor_EmailInvalido_LancaArgumentException(string emailInvalido)
    {
        // Arrange
        Action acao = () => new Tutor("Ana", emailInvalido, "11999999999", null);

        // Act & Assert
        acao.Should().Throw<ArgumentException>().WithMessage("*O email do tutor é inválido.*");
    }

    [Fact]
    public void Construtor_TelefoneVazio_LancaArgumentException()
    {
        // Arrange
        Action acao = () => new Tutor("Ana", "ana@email.com", "", null);

        // Act & Assert
        acao.Should().Throw<ArgumentException>().WithMessage("*O telefone do tutor é obrigatório*");
    }

    [Fact]
    public void Atualizar_DadosValidos_AlteraDados()
    {
        // Arrange
        var tutor = new Tutor("Ana", "ana@email.com", "11999999999", null);

        // Act
        tutor.Atualizar("Ana Lima", "ana.lima@email.com", "11888888888", "Rua B");

        // Assert
        tutor.Nome.Should().Be("Ana Lima");
        tutor.Email.Should().Be("ana.lima@email.com");
        tutor.Endereco.Should().Be("Rua B");
    }

    [Fact]
    public void Atualizar_EmailInvalido_LancaArgumentExceptionEMantemDados()
    {
        // Arrange
        var tutor = new Tutor("Ana", "ana@email.com", "11999999999", null);

        // Act
        Action acao = () => tutor.Atualizar("Ana", "invalido", "11999999999", null);

        // Assert
        acao.Should().Throw<ArgumentException>();
        tutor.Email.Should().Be("ana@email.com");
    }
}
