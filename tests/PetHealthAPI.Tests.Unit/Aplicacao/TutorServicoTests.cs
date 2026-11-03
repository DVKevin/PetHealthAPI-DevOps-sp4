using FluentAssertions;
using Moq;
using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Aplicacao.Excecoes;
using PetHealthAPI.Aplicacao.Servicos;
using PetHealthAPI.Dominio.Comum;
using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Filtros;
using PetHealthAPI.Dominio.Interfaces;
using Xunit;

namespace PetHealthAPI.Tests.Unit.Aplicacao;

public class TutorServicoTests
{
    private readonly Mock<ITutorRepositorio> _tutorRepositorioMock = new();
    private readonly TutorServico _servico;

    public TutorServicoTests()
    {
        _servico = new TutorServico(_tutorRepositorioMock.Object);
    }

    private static TutorRequest CriarRequest(string email = "ana@email.com") =>
        new("Ana", email, "11999999999", "Rua A");

    [Fact]
    public async Task CriarAsync_EmailDisponivel_AdicionaTutor()
    {
        // Arrange
        _tutorRepositorioMock.Setup(r => r.EmailEmUsoAsync("ana@email.com", null)).ReturnsAsync(false);

        // Act
        var resultado = await _servico.CriarAsync(CriarRequest());

        // Assert
        resultado.Email.Should().Be("ana@email.com");
        _tutorRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Tutor>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_EmailEmUso_LancaConflitoExceptionSemGravar()
    {
        // Arrange
        _tutorRepositorioMock.Setup(r => r.EmailEmUsoAsync("ana@email.com", null)).ReturnsAsync(true);

        // Act
        var acao = async () => await _servico.CriarAsync(CriarRequest());

        // Assert
        await acao.Should().ThrowAsync<ConflitoException>().WithMessage(TutorServico.MensagemEmailDuplicado);
        _tutorRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Tutor>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_EmailInvalido_LancaArgumentExceptionSemConsultarRepositorio()
    {
        // Arrange
        var request = CriarRequest(email: "invalido");

        // Act
        var acao = async () => await _servico.CriarAsync(request);

        // Assert
        await acao.Should().ThrowAsync<ArgumentException>().WithMessage("*O email do tutor é inválido.*");
        _tutorRepositorioMock.Verify(r => r.EmailEmUsoAsync(It.IsAny<string>(), It.IsAny<int?>()), Times.Never);
    }

    [Fact]
    public async Task ObterPorIdAsync_TutorInexistente_LancaRecursoNaoEncontradoException()
    {
        // Arrange
        _tutorRepositorioMock.Setup(r => r.ObterPorIdAsync(42)).ReturnsAsync((Tutor?)null);

        // Act
        var acao = async () => await _servico.ObterPorIdAsync(42);

        // Assert
        await acao.Should().ThrowAsync<RecursoNaoEncontradoException>().WithMessage("Tutor com ID 42 não encontrado.");
    }

    [Fact]
    public async Task AtualizarAsync_EmailDeOutroTutor_LancaConflitoException()
    {
        // Arrange
        var tutor = new Tutor("Ana", "ana@email.com", "11999999999", null);
        _tutorRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(tutor);
        _tutorRepositorioMock.Setup(r => r.EmailEmUsoAsync("bia@email.com", 1)).ReturnsAsync(true);

        // Act
        var acao = async () => await _servico.AtualizarAsync(1, CriarRequest(email: "bia@email.com"));

        // Assert
        await acao.Should().ThrowAsync<ConflitoException>();
        _tutorRepositorioMock.Verify(r => r.AtualizarAsync(It.IsAny<Tutor>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DadosValidos_GravaAlteracao()
    {
        // Arrange
        var tutor = new Tutor("Ana", "ana@email.com", "11999999999", null);
        _tutorRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(tutor);
        _tutorRepositorioMock.Setup(r => r.EmailEmUsoAsync("ana@email.com", 1)).ReturnsAsync(false);

        // Act
        await _servico.AtualizarAsync(1, new TutorRequest("Ana Lima", "ana@email.com", "11888888888", null));

        // Assert
        tutor.Nome.Should().Be("Ana Lima");
        _tutorRepositorioMock.Verify(r => r.AtualizarAsync(tutor), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_TutorExistente_RemoveDoRepositorio()
    {
        // Arrange
        var tutor = new Tutor("Ana", "ana@email.com", "11999999999", null);
        _tutorRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(tutor);

        // Act
        await _servico.RemoverAsync(1);

        // Assert
        _tutorRepositorioMock.Verify(r => r.RemoverAsync(tutor), Times.Once);
    }

    [Fact]
    public async Task ListarAsync_ResultadoDoRepositorio_ConverteParaResponseMantendoPaginacao()
    {
        // Arrange
        var filtro = new FiltroTutor { Pagina = 1, TamanhoPagina = 10 };
        var tutores = new List<Tutor> { new("Ana", "ana@email.com", "11999999999", null) };
        _tutorRepositorioMock.Setup(r => r.ListarAsync(filtro))
            .ReturnsAsync(new ResultadoPaginado<Tutor>(tutores, 1, 10, 1));

        // Act
        var resultado = await _servico.ListarAsync(filtro);

        // Assert
        resultado.TotalItens.Should().Be(1);
        resultado.Itens[0].Nome.Should().Be("Ana");
    }
}
