using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PetHealthAPI.Infraestrutura.Health;

/// <summary>Sinaliza "Degraded" quando a memória alocada pelo processo passa do limite.</summary>
public class MemoriaHealthCheck : IHealthCheck
{
    public const long LimiteBytes = 1024L * 1024L * 1024L; // 1 GB

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var memoriaAlocada = GC.GetTotalMemory(forceFullCollection: false);
        var dados = new Dictionary<string, object>
        {
            { "MemoriaAlocadaMB", memoriaAlocada / 1024 / 1024 },
            { "LimiteMB", LimiteBytes / 1024 / 1024 }
        };

        var resultado = memoriaAlocada < LimiteBytes
            ? HealthCheckResult.Healthy("Uso de memória dentro do limite.", dados)
            : HealthCheckResult.Degraded("Uso de memória acima do limite.", data: dados);

        return Task.FromResult(resultado);
    }
}
