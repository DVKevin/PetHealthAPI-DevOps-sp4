using FluentAssertions;
using Moq;
using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Aplicacao.Excecoes;
using PetHealthAPI.Aplicacao.Servicos;
using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Interfaces;
using Xunit;

namespace PetHealthAPI.Tests.Unit.Aplicacao;

public class RegistroDiarioServicoTests
{
    private readonly Mock<IRegistroDiarioRepositorio> _registroRepositorioMock = new();
    private readonly Mock<IPetRepositorio> _petRepositorioMock = new();
    private readonly RegistroDiarioServico _servico;

    public RegistroDiarioServicoTests()
    {
        _servico = new RegistroDiarioServico(_registroRepositorioMock.Object, _petRepositorioMock.Object);
    }

    [Fact]
    public async Task CriarAsync_PetExistente_GravaRegistroComPontosCalculados()
    {
        // Arrange
        _petRepositorioMock.Setup(r => r.ExisteAsync(1)).ReturnsAsync(true);
        var request = new CriarRegistroDiarioRequest(1, DateTime.Today, true, true, true, false, "Feliz", null);

        // Act
        var resultado = await _servico.CriarAsync(request);

        // Assert
        resultado.Pontos.Should().Be(30);
        _registroRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<RegistroDiario>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_PetInexistente_LancaArgumentExceptionSemGravar()
    {
        // Arrange
        _petRepositorioMock.Setup(r => r.ExisteAsync(8)).ReturnsAsync(false);
        var request = new CriarRegistroDiarioRequest(8, DateTime.Today, true, false, false, false, null, null);

        // Act
        var acao = async () => await _servico.CriarAsync(request);

        // Assert
        await acao.Should().ThrowAsync<ArgumentException>().WithMessage("*Pet com ID 8 não encontrado.*");
        _registroRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<RegistroDiario>()), Times.Never);
    }

    [Theory]
    [InlineData(0, 1, 100)]
    [InlineData(250, 3, 50)]
    [InlineData(300, 4, 100)]
    public async Task ObterResumoPontosAsync_TotalDePontos_CalculaNivelEPontosRestantes(int total, int nivelEsperado,
                                                                                        int faltamEsperado)
    {
        // Arrange
        _petRepositorioMock.Setup(r => r.ExisteAsync(1)).ReturnsAsync(true);
        _registroRepositorioMock.Setup(r => r.SomarPontosDoPetAsync(1)).ReturnsAsync(total);

        // Act
        var resumo = await _servico.ObterResumoPontosAsync(1);

        // Assert
        resumo.TotalPontos.Should().Be(total);
        resumo.Nivel.Should().Be(nivelEsperado);
        resumo.PontosParaProximoNivel.Should().Be(faltamEsperado);
    }

    [Fact]
    public async Task ObterResumoPontosAsync_PetInexistente_LancaRecursoNaoEncontradoException()
    {
        // Arrange
        _petRepositorioMock.Setup(r => r.ExisteAsync(8)).ReturnsAsync(false);

        // Act
        var acao = async () => await _servico.ObterResumoPontosAsync(8);

        // Assert
        await acao.Should().ThrowAsync<RecursoNaoEncontradoException>().WithMessage("Pet com ID 8 não encontrado.");
    }

    [Fact]
    public async Task ObterPorIdAsync_RegistroInexistente_LancaRecursoNaoEncontradoException()
    {
        // Arrange
        const string id = "650000000000000000000001";
        _registroRepositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((RegistroDiario?)null);

        // Act
        var acao = async () => await _servico.ObterPorIdAsync(id);

        // Assert
        await acao.Should().ThrowAsync<RecursoNaoEncontradoException>();
    }
}
