using FluentAssertions;
using PetHealthAPI.Dominio.Entidades;
using Xunit;

namespace PetHealthAPI.Tests.Unit.Dominio;

public class ConsultaTests
{
    private static readonly DateTime DataConsulta = new(2026, 8, 20);

    private static Consulta CriarConsulta(string motivo = "Check-up", decimal? custo = 150,
                                          DateTime? retorno = null, int petId = 1)
    {
        return new Consulta(DataConsulta, motivo, "Dr. João", "Clínica Vida", null, null, custo, retorno, null, petId);
    }

    [Fact]
    public void Construtor_DadosValidos_CriaConsulta()
    {
        // Arrange & Act
        var consulta = CriarConsulta();

        // Assert
        consulta.Motivo.Should().Be("Check-up");
        consulta.Custo.Should().Be(150);
    }

    [Fact]
    public void Construtor_MotivoVazio_LancaArgumentException()
    {
        // Arrange
        Action acao = () => CriarConsulta(motivo: " ");

        // Act & Assert
        acao.Should().Throw<ArgumentException>().WithMessage("*O motivo da consulta é obrigatório.*");
    }

    [Fact]
    public void Construtor_CustoNegativo_LancaArgumentException()
    {
        // Arrange
        Action acao = () => CriarConsulta(custo: -1);

        // Act & Assert
        acao.Should().Throw<ArgumentException>().WithMessage("*O custo da consulta não pode ser negativo.*");
    }

    [Fact]
    public void Construtor_RetornoAntesDaConsulta_LancaArgumentException()
    {
        // Arrange
        Action acao = () => CriarConsulta(retorno: DataConsulta.AddDays(-2));

        // Act & Assert
        acao.Should().Throw<ArgumentException>()
            .WithMessage("*A data de retorno não pode ser anterior à data da consulta.*");
    }

    [Theory]
    [InlineData(5, true)]
    [InlineData(0, true)]
    [InlineData(-5, false)]
    public void TemRetornoPendente_DataDeRetorno_RetornaSeAindaVaiAcontecer(int diasAteRetorno, bool esperado)
    {
        // Arrange
        var referencia = DataConsulta.AddDays(10);
        var consulta = CriarConsulta(retorno: referencia.AddDays(diasAteRetorno));

        // Act
        var resultado = consulta.TemRetornoPendente(referencia);

        // Assert
        resultado.Should().Be(esperado);
    }
}
