using Microsoft.Extensions.Diagnostics.HealthChecks;
using PetHealthAPI.Infraestrutura.Persistencia;

namespace PetHealthAPI.Infraestrutura.Health;

/// <summary>Verifica se a API consegue abrir conexão com o Oracle via EF Core.</summary>
public class OracleHealthCheck : IHealthCheck
{
    private readonly AppDbContext _contexto;

    public OracleHealthCheck(AppDbContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var conectou = await _contexto.Database.CanConnectAsync(cancellationToken);

            return conectou
                ? HealthCheckResult.Healthy("Conexão com o Oracle estabelecida.")
                : new HealthCheckResult(context.Registration.FailureStatus, "Não foi possível conectar ao Oracle.");
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, "Falha ao verificar o Oracle.", ex);
        }
    }
}
