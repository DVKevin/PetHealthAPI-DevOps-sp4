using FluentAssertions;
using Moq;
using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Aplicacao.Excecoes;
using PetHealthAPI.Aplicacao.Servicos;
using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Interfaces;
using Xunit;

namespace PetHealthAPI.Tests.Unit.Aplicacao;

public class VacinaServicoTests
{
    private readonly Mock<IVacinaRepositorio> _vacinaRepositorioMock = new();
    private readonly Mock<IPetRepositorio> _petRepositorioMock = new();
    private readonly VacinaServico _servico;

    public VacinaServicoTests()
    {
        _servico = new VacinaServico(_vacinaRepositorioMock.Object, _petRepositorioMock.Object);
    }

    private static CriarVacinaRequest CriarRequest(int petId = 1) =>
        new("V10", new DateTime(2026, 5, 10), new DateTime(2027, 5, 10), "Zoetis", "L1", "Dra. Paula", null, petId);

    [Fact]
    public async Task CriarAsync_PetExistente_AdicionaVacina()
    {
        // Arrange
        _petRepositorioMock.Setup(r => r.ExisteAsync(1)).ReturnsAsync(true);

        // Act
        var resultado = await _servico.CriarAsync(CriarRequest());

        // Assert
        resultado.Nome.Should().Be("V10");
        _vacinaRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Vacina>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_PetInexistente_LancaArgumentExceptionSemGravar()
    {
        // Arrange
        _petRepositorioMock.Setup(r => r.ExisteAsync(99)).ReturnsAsync(false);

        // Act
        var acao = async () => await _servico.CriarAsync(CriarRequest(petId: 99));

        // Assert
        await acao.Should().ThrowAsync<ArgumentException>().WithMessage("*Pet com ID 99 não encontrado.*");
        _vacinaRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Vacina>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_VacinaInexistente_LancaRecursoNaoEncontradoException()
    {
        // Arrange
        _vacinaRepositorioMock.Setup(r => r.ObterPorIdAsync(5)).ReturnsAsync((Vacina?)null);
        var request = new AtualizarVacinaRequest("V10", new DateTime(2026, 5, 10), null, null, null, null, null);

        // Act
        var acao = async () => await _servico.AtualizarAsync(5, request);

        // Assert
        await acao.Should().ThrowAsync<RecursoNaoEncontradoException>().WithMessage("Vacina com ID 5 não encontrada.");
    }

    [Fact]
    public async Task RemoverAsync_VacinaExistente_RemoveDoRepositorio()
    {
        // Arrange
        var vacina = new Vacina("V10", new DateTime(2026, 5, 10), null, null, null, null, null, 1);
        _vacinaRepositorioMock.Setup(r => r.ObterPorIdAsync(5)).ReturnsAsync(vacina);

        // Act
        await _servico.RemoverAsync(5);

        // Assert
        _vacinaRepositorioMock.Verify(r => r.RemoverAsync(vacina), Times.Once);
    }
}
