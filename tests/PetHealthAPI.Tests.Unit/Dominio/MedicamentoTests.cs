using FluentAssertions;
using PetHealthAPI.Dominio.Entidades;
using Xunit;

namespace PetHealthAPI.Tests.Unit.Dominio;

public class MedicamentoTests
{
    private static readonly DateTime DataInicio = new(2026, 9, 1);

    private static Medicamento CriarMedicamento(string nome = "Amoxicilina", string dosagem = "250mg",
                                                string frequencia = "12/12h", DateTime? dataFim = null)
    {
        return new Medicamento(nome, dosagem, frequencia, DataInicio, dataFim, "Dra. Paula", null, 1);
    }

    [Fact]
    public void Construtor_DadosValidos_NasceAtivo()
    {
        // Arrange & Act
        var medicamento = CriarMedicamento();

        // Assert
        medicamento.Ativo.Should().BeTrue();
        medicamento.DataFim.Should().BeNull();
    }

    [Theory]
    [InlineData("", "250mg", "12/12h", "*O nome do medicamento é obrigatório.*")]
    [InlineData("Amoxicilina", "", "12/12h", "*A dosagem do medicamento é obrigatória.*")]
    [InlineData("Amoxicilina", "250mg", "", "*A frequência do medicamento é obrigatória.*")]
    public void Construtor_CampoObrigatorioVazio_LancaArgumentException(string nome, string dosagem,
                                                                       string frequencia, string mensagem)
    {
        // Arrange
        Action acao = () => CriarMedicamento(nome, dosagem, frequencia);

        // Act & Assert
        acao.Should().Throw<ArgumentException>().WithMessage(mensagem);
    }

    [Fact]
    public void Construtor_DataFimAntesDoInicio_LancaArgumentException()
    {
        // Arrange
        Action acao = () => CriarMedicamento(dataFim: DataInicio.AddDays(-1));

        // Act & Assert
        acao.Should().Throw<ArgumentException>()
            .WithMessage("*A data de fim não pode ser anterior à data de início.*");
    }

    [Fact]
    public void Encerrar_DataValida_DesativaTratamento()
    {
        // Arrange
        var medicamento = CriarMedicamento();

        // Act
        medicamento.Encerrar(DataInicio.AddDays(7));

        // Assert
        medicamento.Ativo.Should().BeFalse();
        medicamento.DataFim.Should().Be(DataInicio.AddDays(7));
    }

    [Fact]
    public void Encerrar_DataAnteriorAoInicio_LancaArgumentExceptionEMantemAtivo()
    {
        // Arrange
        var medicamento = CriarMedicamento();

        // Act
        Action acao = () => medicamento.Encerrar(DataInicio.AddDays(-1));

        // Assert
        acao.Should().Throw<ArgumentException>();
        medicamento.Ativo.Should().BeTrue();
    }
}
