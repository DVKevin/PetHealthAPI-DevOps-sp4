using FluentAssertions;
using PetHealthAPI.Dominio.Comum;
using PetHealthAPI.Dominio.Filtros;
using Xunit;

namespace PetHealthAPI.Tests.Unit.Dominio;

public class ComumTests
{
    [Theory]
    [InlineData(0, 1)]
    [InlineData(-3, 1)]
    [InlineData(4, 4)]
    public void Pagina_ValorInformado_NuncaFicaMenorQueUm(int informado, int esperado)
    {
        // Arrange
        var filtro = new FiltroPet();

        // Act
        filtro.Pagina = informado;

        // Assert
        filtro.Pagina.Should().Be(esperado);
    }

    [Theory]
    [InlineData(0, ParametrosConsulta.TamanhoPaginaPadrao)]
    [InlineData(500, ParametrosConsulta.TamanhoPaginaMaximo)]
    [InlineData(20, 20)]
    public void TamanhoPagina_ValorInformado_RespeitaLimites(int informado, int esperado)
    {
        // Arrange
        var filtro = new FiltroPet();

        // Act
        filtro.TamanhoPagina = informado;

        // Assert
        filtro.TamanhoPagina.Should().Be(esperado);
    }

    [Theory]
    [InlineData("desc", true)]
    [InlineData("DESC", true)]
    [InlineData("asc", false)]
    [InlineData(null, false)]
    public void EhDescendente_Direcao_InterpretaSemDiferenciarMaiusculas(string? direcao, bool esperado)
    {
        // Arrange
        var filtro = new FiltroPet { Direcao = direcao };

        // Act
        var resultado = filtro.EhDescendente();

        // Assert
        resultado.Should().Be(esperado);
    }

    [Fact]
    public void ResultadoPaginado_PaginaDoMeio_CalculaNavegacao()
    {
        // Arrange & Act
        var resultado = new ResultadoPaginado<int>(new[] { 1, 2 }, pagina: 2, tamanhoPagina: 10, totalItens: 25);

        // Assert
        resultado.TotalPaginas.Should().Be(3);
        resultado.TemPaginaAnterior.Should().BeTrue();
        resultado.TemProximaPagina.Should().BeTrue();
    }

    [Fact]
    public void Mapear_Conversor_MantemMetadadosDaPagina()
    {
        // Arrange
        var resultado = new ResultadoPaginado<int>(new[] { 1, 2 }, 1, 2, 2);

        // Act
        var convertido = resultado.Mapear(n => $"item-{n}");

        // Assert
        convertido.Itens.Should().Equal("item-1", "item-2");
        convertido.TotalItens.Should().Be(2);
        convertido.TemProximaPagina.Should().BeFalse();
    }

    [Theory]
    [InlineData("ana@email.com", true)]
    [InlineData("ana.lima@empresa.com.br", true)]
    [InlineData("ana@@email.com", false)]
    [InlineData("ana@email.", false)]
    [InlineData(null, false)]
    public void ValidadorEmail_Formato_RetornaSeEhValido(string? email, bool esperado)
    {
        // Act
        var resultado = ValidadorEmail.EhValido(email);

        // Assert
        resultado.Should().Be(esperado);
    }
}
