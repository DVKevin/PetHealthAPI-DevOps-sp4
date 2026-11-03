using FluentAssertions;
using Moq;
using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Aplicacao.Excecoes;
using PetHealthAPI.Aplicacao.Servicos;
using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Interfaces;
using Xunit;

namespace PetHealthAPI.Tests.Unit.Aplicacao;

public class ConsultaServicoTests
{
    private readonly Mock<IConsultaRepositorio> _consultaRepositorioMock = new();
    private readonly Mock<IPetRepositorio> _petRepositorioMock = new();
    private readonly ConsultaServico _servico;

    public ConsultaServicoTests()
    {
        _servico = new ConsultaServico(_consultaRepositorioMock.Object, _petRepositorioMock.Object);
    }

    private static CriarConsultaRequest CriarRequest(decimal? custo = 150) =>
        new(new DateTime(2026, 8, 20), "Check-up", "Dr. João", "Clínica Vida", null, null, custo, null, null, 1);

    [Fact]
    public async Task CriarAsync_PetExistente_AdicionaConsulta()
    {
        // Arrange
        _petRepositorioMock.Setup(r => r.ExisteAsync(1)).ReturnsAsync(true);

        // Act
        var resultado = await _servico.CriarAsync(CriarRequest());

        // Assert
        resultado.Motivo.Should().Be("Check-up");
        _consultaRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Consulta>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_CustoNegativo_LancaArgumentExceptionSemConsultarPet()
    {
        // Arrange
        var request = CriarRequest(custo: -10);

        // Act
        var acao = async () => await _servico.CriarAsync(request);

        // Assert
        await acao.Should().ThrowAsync<ArgumentException>().WithMessage("*O custo da consulta não pode ser negativo.*");
        _petRepositorioMock.Verify(r => r.ExisteAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task ObterPorIdAsync_ConsultaInexistente_LancaRecursoNaoEncontradoException()
    {
        // Arrange
        _consultaRepositorioMock.Setup(r => r.ObterPorIdAsync(7)).ReturnsAsync((Consulta?)null);

        // Act
        var acao = async () => await _servico.ObterPorIdAsync(7);

        // Assert
        await acao.Should().ThrowAsync<RecursoNaoEncontradoException>().WithMessage("Consulta com ID 7 não encontrada.");
    }

    [Fact]
    public async Task AtualizarAsync_DadosValidos_GravaAlteracao()
    {
        // Arrange
        var consulta = new Consulta(new DateTime(2026, 8, 20), "Check-up", null, null, null, null, 100, null, null, 1);
        _consultaRepositorioMock.Setup(r => r.ObterPorIdAsync(7)).ReturnsAsync(consulta);
        var request = new AtualizarConsultaRequest(new DateTime(2026, 8, 20), "Retorno", null, null,
                                                   "Otite", "Antibiótico", 200, null, null);

        // Act
        await _servico.AtualizarAsync(7, request);

        // Assert
        consulta.Diagnostico.Should().Be("Otite");
        _consultaRepositorioMock.Verify(r => r.AtualizarAsync(consulta), Times.Once);
    }
}
