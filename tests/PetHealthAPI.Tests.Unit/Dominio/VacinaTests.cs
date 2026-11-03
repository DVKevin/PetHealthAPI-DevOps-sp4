using FluentAssertions;
using PetHealthAPI.Dominio.Entidades;
using Xunit;

namespace PetHealthAPI.Tests.Unit.Dominio;

public class VacinaTests
{
    private static readonly DateTime DataAplicacao = new(2026, 5, 10);

    private static Vacina CriarVacina(string nome = "V10", DateTime? proximaDose = null, int petId = 1)
    {
        return new Vacina(nome, DataAplicacao, proximaDose, "Zoetis", "L123", "Dra. Paula", null, petId);
    }

    [Fact]
    public void Construtor_DadosValidos_CriaVacina()
    {
        // Arrange & Act
        var vacina = CriarVacina(proximaDose: DataAplicacao.AddYears(1));

        // Assert
        vacina.Nome.Should().Be("V10");
        vacina.PetId.Should().Be(1);
    }

    [Fact]
    public void Construtor_NomeVazio_LancaArgumentException()
    {
        // Arrange
        Action acao = () => CriarVacina(nome: "");

        // Act & Assert
        acao.Should().Throw<ArgumentException>().WithMessage("*O nome da vacina é obrigatório.*");
    }

    [Fact]
    public void Construtor_ProximaDoseAntesDaAplicacao_LancaArgumentException()
    {
        // Arrange
        Action acao = () => CriarVacina(proximaDose: DataAplicacao.AddDays(-1));

        // Act & Assert
        acao.Should().Throw<ArgumentException>()
            .WithMessage("*A data da próxima dose não pode ser anterior à data de aplicação.*");
    }

    [Fact]
    public void Construtor_SemPet_LancaArgumentException()
    {
        // Arrange
        Action acao = () => CriarVacina(petId: 0);

        // Act & Assert
        acao.Should().Throw<ArgumentException>().WithMessage("*O pet da vacina deve ser informado.*");
    }

    [Theory]
    [InlineData(10, true)]
    [InlineData(30, true)]
    [InlineData(31, false)]
    [InlineData(-1, false)]
    public void ProximaDoseVenceEm_DiasAteAProximaDose_RetornaSeEstaNoPeriodo(int diasAteDose, bool esperado)
    {
        // Arrange
        var hoje = new DateTime(2026, 9, 1);
        var vacina = new Vacina("V10", hoje.AddDays(-400), hoje.AddDays(diasAteDose), null, null, null, null, 1);

        // Act
        var resultado = vacina.ProximaDoseVenceEm(30, hoje);

        // Assert
        resultado.Should().Be(esperado);
    }

    [Fact]
    public void ProximaDoseVenceEm_SemProximaDose_RetornaFalse()
    {
        // Arrange
        var vacina = CriarVacina(proximaDose: null);

        // Act
        var resultado = vacina.ProximaDoseVenceEm(30, DataAplicacao);

        // Assert
        resultado.Should().BeFalse();
    }
}
