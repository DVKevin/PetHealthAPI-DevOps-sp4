using FluentAssertions;
using Moq;
using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Aplicacao.Excecoes;
using PetHealthAPI.Aplicacao.Servicos;
using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Interfaces;
using Xunit;

namespace PetHealthAPI.Tests.Unit.Aplicacao;

public class PetServicoTests
{
    private readonly Mock<IPetRepositorio> _petRepositorioMock = new();
    private readonly Mock<ITutorRepositorio> _tutorRepositorioMock = new();
    private readonly PetServico _servico;

    public PetServicoTests()
    {
        _servico = new PetServico(_petRepositorioMock.Object, _tutorRepositorioMock.Object);
    }

    private static PetRequest CriarRequest(string nome = "Rex", int tutorId = 1)
    {
        return new PetRequest(nome, "Cão", "Vira-lata", 3, 15, "Macho", true, null, null, tutorId);
    }

    [Fact]
    public async Task CriarAsync_TutorExistenteEDadosValidos_AdicionaPet()
    {
        // Arrange
        var request = CriarRequest();
        _tutorRepositorioMock.Setup(r => r.ExisteAsync(request.TutorId)).ReturnsAsync(true);

        // Act
        var resultado = await _servico.CriarAsync(request);

        // Assert
        resultado.Nome.Should().Be("Rex");
        _petRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Pet>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_TutorInexistente_LancaArgumentExceptionSemGravar()
    {
        // Arrange
        var request = CriarRequest();
        _tutorRepositorioMock.Setup(r => r.ExisteAsync(request.TutorId)).ReturnsAsync(false);

        // Act
        var acao = async () => await _servico.CriarAsync(request);

        // Assert
        await acao.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Tutor com ID 1 não encontrado. Cadastre o tutor antes do pet.*");
        _petRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Pet>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_NomeInvalido_LancaArgumentExceptionSemConsultarRepositorio()
    {
        // Arrange
        var request = CriarRequest(nome: "");

        // Act
        var acao = async () => await _servico.CriarAsync(request);

        // Assert
        await acao.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*O nome do pet é obrigatório.*");
        _tutorRepositorioMock.Verify(r => r.ExisteAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task ObterPorIdAsync_PetInexistente_LancaRecursoNaoEncontradoException()
    {
        // Arrange
        _petRepositorioMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Pet?)null);

        // Act
        var acao = async () => await _servico.ObterPorIdAsync(99);

        // Assert
        await acao.Should().ThrowAsync<RecursoNaoEncontradoException>()
            .WithMessage("Pet com ID 99 não encontrado.");
    }

    [Fact]
    public async Task AtualizarAsync_TrocaParaTutorInexistente_LancaArgumentExceptionSemGravar()
    {
        // Arrange
        var pet = new Pet("Rex", "Cão", null, 3, 15, null, false, null, null, 1);
        _petRepositorioMock.Setup(r => r.ObterPorIdAsync(10)).ReturnsAsync(pet);
        _tutorRepositorioMock.Setup(r => r.ExisteAsync(2)).ReturnsAsync(false);

        // Act
        var acao = async () => await _servico.AtualizarAsync(10, CriarRequest(tutorId: 2));

        // Assert
        await acao.Should().ThrowAsync<ArgumentException>().WithMessage("*Tutor com ID 2 não encontrado*");
        _petRepositorioMock.Verify(r => r.AtualizarAsync(It.IsAny<Pet>()), Times.Never);
    }

    [Fact]
    public async Task RemoverAsync_PetExistente_RemoveDoRepositorio()
    {
        // Arrange
        var pet = new Pet("Rex", "Cão", null, 3, 15, null, false, null, null, 1);
        _petRepositorioMock.Setup(r => r.ObterPorIdAsync(10)).ReturnsAsync(pet);

        // Act
        await _servico.RemoverAsync(10);

        // Assert
        _petRepositorioMock.Verify(r => r.RemoverAsync(pet), Times.Once);
    }
}
