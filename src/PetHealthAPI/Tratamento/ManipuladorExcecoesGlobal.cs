using Microsoft.AspNetCore.Diagnostics;
using PetHealthAPI.Aplicacao.Excecoes;

namespace PetHealthAPI.Tratamento;

/// <summary>
/// Tratamento global e consistente de exceções: toda falha vira um ProblemDetails (RFC 7807)
/// com status HTTP adequado, correlationId e traceId. Os controllers não têm try/catch de negócio.
/// </summary>
public class ManipuladorExcecoesGlobal : IExceptionHandler
{
    private readonly ILogger<ManipuladorExcecoesGlobal> _logger;

    public ManipuladorExcecoesGlobal(ILogger<ManipuladorExcecoesGlobal> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, titulo, detalhe) = Classificar(exception);

        if (status >= StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Erro não tratado em {Metodo} {Caminho}",
                httpContext.Request.Method, httpContext.Request.Path);
        else
            _logger.LogWarning("{Titulo} em {Metodo} {Caminho}: {Detalhe}",
                titulo, httpContext.Request.Method, httpContext.Request.Path, detalhe);

        await EscritorProblemDetails.EscreverAsync(httpContext, status, titulo, detalhe, cancellationToken);
        return true;
    }

    public static (int Status, string Titulo, string Detalhe) Classificar(Exception exception) => exception switch
    {
        RecursoNaoEncontradoException e => (StatusCodes.Status404NotFound, "Recurso não encontrado", e.Message),
        ConflitoException e => (StatusCodes.Status409Conflict, "Conflito", e.Message),
        CredenciaisInvalidasException e => (StatusCodes.Status401Unauthorized, "Não autorizado", e.Message),
        ArgumentException e => (StatusCodes.Status400BadRequest, "Requisição inválida", MensagemSemParametro(e)),
        _ => (StatusCodes.Status500InternalServerError, "Erro interno",
              "Ocorreu um erro inesperado. Informe o correlationId ao suporte.")
    };

    /// <summary>Remove o sufixo " (Parameter 'x')" que o .NET adiciona quando o nameof() é informado.</summary>
    private static string MensagemSemParametro(ArgumentException exception)
    {
        if (string.IsNullOrEmpty(exception.ParamName))
            return exception.Message;

        var sufixo = $" (Parameter '{exception.ParamName}')";
        return exception.Message.EndsWith(sufixo, StringComparison.Ordinal)
            ? exception.Message[..^sufixo.Length]
            : exception.Message;
    }
}
