using FluentAssertions;
using PetHealthAPI.Dominio.Entidades;
using Xunit;

namespace PetHealthAPI.Tests.Unit.Dominio;

public class PetTests
{
    private static Pet CriarPet(string nome = "Rex", string especie = "Cão", int? idade = 3,
                                decimal? peso = 15, int tutorId = 1)
    {
        return new Pet(nome, especie, "Vira-lata", idade, peso, "Macho", true, null, null, tutorId);
    }

    [Fact]
    public void Construtor_DadosValidos_CriaPetComDadosInformados()
    {
        // Arrange & Act
        var pet = CriarPet();

        // Assert
        pet.Nome.Should().Be("Rex");
        pet.Especie.Should().Be("Cão");
        pet.TutorId.Should().Be(1);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Construtor_NomeInvalido_LancaArgumentException(string nomeInvalido)
    {
        // Arrange
        Action acao = () => CriarPet(nome: nomeInvalido);

        // Act & Assert
        acao.Should().Throw<ArgumentException>()
            .WithMessage("*O nome do pet é obrigatório.*");
    }

    [Fact]
    public void Construtor_IdadeNegativa_LancaArgumentException()
    {
        // Arrange
        Action acao = () => CriarPet(idade: -1);

        // Act & Assert
        acao.Should().Throw<ArgumentException>()
            .WithMessage("*A idade do pet não pode ser negativa.*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Construtor_PesoMenorOuIgualAZero_LancaArgumentException(double pesoInvalido)
    {
        // Arrange
        Action acao = () => CriarPet(peso: (decimal)pesoInvalido);

        // Act & Assert
        acao.Should().Throw<ArgumentException>()
            .WithMessage("*O peso do pet deve ser maior que zero.*");
    }

    [Fact]
    public void Atualizar_DadosValidos_AlteraOsDados()
    {
        // Arrange
        var pet = CriarPet();

        // Act
        pet.Atualizar("Thor", "Cão", "Labrador", 4, 30, "Macho", false, "Frango", null, 2);

        // Assert
        pet.Nome.Should().Be("Thor");
        pet.Peso.Should().Be(30);
        pet.TutorId.Should().Be(2);
    }
}
