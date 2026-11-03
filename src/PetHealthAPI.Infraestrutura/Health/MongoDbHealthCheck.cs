using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace PetHealthAPI.Infraestrutura.Health;

/// <summary>Envia o comando "ping" ao MongoDB.</summary>
public class MongoDbHealthCheck : IHealthCheck
{
    private static readonly TimeSpan TempoLimite = TimeSpan.FromSeconds(3);

    private readonly IMongoDatabase _banco;

    public MongoDbHealthCheck(IMongoDatabase banco)
    {
        _banco = banco;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var limite = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            limite.CancelAfter(TempoLimite);

            await _banco.RunCommandAsync((Command<BsonDocument>)"{ ping: 1 }", cancellationToken: limite.Token);

            return HealthCheckResult.Healthy("MongoDB respondeu ao ping.");
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, "MongoDB indisponível.", ex);
        }
    }
}
