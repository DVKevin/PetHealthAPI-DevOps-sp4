using System.Diagnostics;
using Serilog.Context;

namespace PetHealthAPI.Middlewares;

/// <summary>
/// Garante um X-Correlation-ID por requisição: reaproveita o do cliente ou gera um novo,
/// injeta no LogContext do Serilog, no Span atual e devolve no header da resposta.
/// </summary>
public class CorrelationIdMiddleware
{
    public const string NomeHeader = "X-Correlation-ID";
    public const string ChaveItem = "CorrelationId";

    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers.TryGetValue(NomeHeader, out var valorRecebido) &&
                            !string.IsNullOrWhiteSpace(valorRecebido)
            ? valorRecebido.ToString()
            : Guid.NewGuid().ToString();

        context.Items[ChaveItem] = correlationId;
        Activity.Current?.SetTag("correlation.id", correlationId);

        // OnStarting: o header é gravado mesmo quando a resposta é gerada pelo tratamento global de exceções
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[NomeHeader] = correlationId;
            return Task.CompletedTask;
        });

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}
