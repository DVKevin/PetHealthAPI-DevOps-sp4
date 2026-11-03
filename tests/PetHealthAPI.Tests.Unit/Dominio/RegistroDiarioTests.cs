using FluentAssertions;
using PetHealthAPI.Dominio.Entidades;
using Xunit;

namespace PetHealthAPI.Tests.Unit.Dominio;

public class RegistroDiarioTests
{
    [Theory]
    [InlineData(false, false, false, false, 0)]
    [InlineData(true, false, false, false, 10)]
    [InlineData(true, true, false, false, 20)]
    [InlineData(true, true, true, true, 40)]
    public void Construtor_CuidadosRealizados_CalculaDezPontosPorCuidado(bool alimentacao, bool hidratacao,
        bool passeio, bool medicacao, int pontosEsperados)
    {
        // Arrange & Act
        var registro = new RegistroDiario(1, DateTime.Today, alimentacao, hidratacao, passeio, medicacao, null, null);

        // Assert
        registro.Pontos.Should().Be(pontosEsperados);
    }

    [Fact]
    public void Construtor_DataFutura_LancaArgumentException()
    {
        // Arrange
        Action acao = () => new RegistroDiario(1, DateTime.Today.AddDays(1), true, true, true, true, null, null);

        // Act & Assert
        acao.Should().Throw<ArgumentException>().WithMessage("*A data do registro não pode ser futura.*");
    }

    [Fact]
    public void Construtor_SemPet_LancaArgumentException()
    {
        // Arrange
        Action acao = () => new RegistroDiario(0, DateTime.Today, true, true, true, true, null, null);

        // Act & Assert
        acao.Should().Throw<ArgumentException>().WithMessage("*O pet do registro deve ser informado.*");
    }

    [Fact]
    public void Construtor_ObservacoesMuitoLongas_LancaArgumentException()
    {
        // Arrange
        var observacoes = new string('x', RegistroDiario.TamanhoMaximoObservacoes + 1);
        Action acao = () => new RegistroDiario(1, DateTime.Today, true, true, true, true, null, observacoes);

        // Act & Assert
        acao.Should().Throw<ArgumentException>().WithMessage("*As observações devem ter no máximo 500 caracteres.*");
    }

    [Fact]
    public void Atualizar_NovosCuidados_RecalculaPontos()
    {
        // Arrange
        var registro = new RegistroDiario(1, DateTime.Today, true, false, false, false, null, null);

        // Act
        registro.Atualizar(DateTime.Today, true, true, true, false, "Feliz", "Passeou no parque");

        // Assert
        registro.Pontos.Should().Be(30);
        registro.Humor.Should().Be("Feliz");
    }

    [Fact]
    public void DefinirId_IdVazio_LancaArgumentException()
    {
        // Arrange
        var registro = new RegistroDiario(1, DateTime.Today, true, true, true, true, null, null);

        // Act
        Action acao = () => registro.DefinirId("");

        // Assert
        acao.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Construtor_DataComHorario_GuardaSomenteODiaEmUtc()
    {
        // Arrange
        var dataComHorario = new DateTime(2026, 9, 29, 21, 45, 0, DateTimeKind.Local);

        // Act
        var registro = new RegistroDiario(1, dataComHorario, true, false, false, false, null, null);

        // Assert
        registro.Data.Should().Be(new DateTime(2026, 9, 29, 0, 0, 0, DateTimeKind.Utc));
        registro.Data.Kind.Should().Be(DateTimeKind.Utc);
    }
}
