using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace PetHealthAPI.Infraestrutura.Observabilidade;

/// <summary>
/// Ponto único de observabilidade customizada: ActivitySource (spans) e Meter (métricas).
/// </summary>
public class AplicacaoMetricas
{
    public const string NomeServico = "PetHealthAPI.API";
    public const string NomeActivitySource = NomeServico;

    public static readonly ActivitySource ActivitySource = new(NomeActivitySource, "1.0.0");

    private readonly Meter _meter;
    private readonly Counter<long> _petsCriados;
    private readonly Counter<long> _registrosDiariosCriados;

    public AplicacaoMetricas()
    {
        _meter = new Meter(NomeServico, "1.0.0");

        _petsCriados = _meter.CreateCounter<long>(
            name: "pets_criados_total",
            unit: "{pets}",
            description: "Total de tentativas de cadastro de pets, por status (sucesso ou erro_validacao).");

        _registrosDiariosCriados = _meter.CreateCounter<long>(
            name: "registros_diarios_criados_total",
            unit: "{registros}",
            description: "Total de check-ins diários gravados no MongoDB, por status.");
    }

    public void RegistrarPetCriado(string status)
    {
        _petsCriados.Add(1, new KeyValuePair<string, object?>("status", status));
    }

    public void RegistrarRegistroDiarioCriado(string status)
    {
        _registrosDiariosCriados.Add(1, new KeyValuePair<string, object?>("status", status));
    }
}
