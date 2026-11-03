using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PetHealthAPI.Configuracoes;

/// <summary>Resposta JSON detalhada do endpoint /health.</summary>
public static class RespostaHealthCheck
{
    private static readonly JsonSerializerOptions OpcoesJson = new()
    {
        WriteIndented = true,
        // Mantém acentos legíveis ("Conexão" em vez de "Conex\u00E3o")
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static Task EscreverAsync(HttpContext context, HealthReport relatorio)
    {
        context.Response.ContentType = "application/json";

        var resposta = new
        {
            status = relatorio.Status.ToString(),
            duracaoTotalMs = relatorio.TotalDuration.TotalMilliseconds,
            checks = relatorio.Entries.Select(entrada => new
            {
                nome = entrada.Key,
                status = entrada.Value.Status.ToString(),
                descricao = entrada.Value.Description,
                erro = entrada.Value.Exception?.Message,
                duracaoMs = entrada.Value.Duration.TotalMilliseconds,
                tags = entrada.Value.Tags,
                dados = entrada.Value.Data
            })
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(resposta, OpcoesJson));
    }
}
