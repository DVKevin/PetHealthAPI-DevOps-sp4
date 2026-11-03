using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PetHealthAPI.Middlewares;

namespace PetHealthAPI.Tratamento;

/// <summary>
/// Formato único de erro da API (RFC 7807), usado pelo tratamento global de exceções
/// e pelas respostas 401/403 da autenticação JWT.
/// </summary>
public static class EscritorProblemDetails
{
    public static Task EscreverAsync(HttpContext httpContext, int status, string titulo, string detalhe,
        CancellationToken cancellationToken = default)
    {
        var problema = new ProblemDetails
        {
            Status = status,
            Title = titulo,
            Detail = detalhe,
            Instance = httpContext.Request.Path
        };
        problema.Extensions["correlationId"] = httpContext.Items[CorrelationIdMiddleware.ChaveItem]?.ToString();
        problema.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = status;
        return httpContext.Response.WriteAsJsonAsync(problema, options: null,
            contentType: "application/problem+json", cancellationToken: cancellationToken);
    }
}
