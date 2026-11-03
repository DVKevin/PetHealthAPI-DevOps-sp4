using FluentAssertions;
using Moq;
using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Aplicacao.Excecoes;
using PetHealthAPI.Aplicacao.Servicos;
using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Interfaces;
using Xunit;

namespace PetHealthAPI.Tests.Unit.Aplicacao;

public class MedicamentoServicoTests
{
    private static readonly DateTime DataInicio = new(2026, 9, 1);

    private readonly Mock<IMedicamentoRepositorio> _medicamentoRepositorioMock = new();
    private readonly Mock<IPetRepositorio> _petRepositorioMock = new();
    private readonly MedicamentoServico _servico;

    public MedicamentoServicoTests()
    {
        _servico = new MedicamentoServico(_medicamentoRepositorioMock.Object, _petRepositorioMock.Object);
    }

    private static Medicamento CriarMedicamento() =>
        new("Amoxicilina", "250mg", "12/12h", DataInicio, null, "Dra. Paula", null, 1);

    [Fact]
    public async Task CriarAsync_PetExistente_AdicionaMedicamentoAtivo()
    {
        // Arrange
        _petRepositorioMock.Setup(r => r.ExisteAsync(1)).ReturnsAsync(true);
        var request = new CriarMedicamentoRequest("Amoxicilina", "250mg", "12/12h", DataInicio, null, null, null, 1);

        // Act
        var resultado = await _servico.CriarAsync(request);

        // Assert
        resultado.Ativo.Should().BeTrue();
        _medicamentoRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Medicamento>()), Times.Once);
    }

    [Fact]
    public async Task EncerrarAsync_DataInformada_EncerraTratamentoEGrava()
    {
        // Arrange
        var medicamento = CriarMedicamento();
        _medicamentoRepositorioMock.Setup(r => r.ObterPorIdAsync(3)).ReturnsAsync(medicamento);

        // Act
        var resultado = await _servico.EncerrarAsync(3, new EncerrarMedicamentoRequest(DataInicio.AddDays(10)));

        // Assert
        resultado.Ativo.Should().BeFalse();
        resultado.DataFim.Should().Be(DataInicio.AddDays(10));
        _medicamentoRepositorioMock.Verify(r => r.AtualizarAsync(medicamento), Times.Once);
    }

    [Fact]
    public async Task EncerrarAsync_SemData_UsaDataAtual()
    {
        // Arrange
        var medicamento = CriarMedicamento();
        _medicamentoRepositorioMock.Setup(r => r.ObterPorIdAsync(3)).ReturnsAsync(medicamento);

        // Act
        var resultado = await _servico.EncerrarAsync(3, new EncerrarMedicamentoRequest(null));

        // Assert
        resultado.Ativo.Should().BeFalse();
        resultado.DataFim!.Value.Date.Should().Be(DateTime.Today);
    }

    [Fact]
    public async Task EncerrarAsync_MedicamentoInexistente_LancaRecursoNaoEncontradoException()
    {
        // Arrange
        _medicamentoRepositorioMock.Setup(r => r.ObterPorIdAsync(3)).ReturnsAsync((Medicamento?)null);

        // Act
        var acao = async () => await _servico.EncerrarAsync(3, new EncerrarMedicamentoRequest(null));

        // Assert
        await acao.Should().ThrowAsync<RecursoNaoEncontradoException>()
            .WithMessage("Medicamento com ID 3 não encontrado.");
    }
}
