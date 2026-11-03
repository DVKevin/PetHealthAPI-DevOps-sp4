using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PetHealthAPI.Configuracoes;
using PetHealthAPI.Infraestrutura;
using PetHealthAPI.Infraestrutura.Observabilidade;
using PetHealthAPI.Middlewares;
using PetHealthAPI.Tratamento;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

const string TemplateLog = "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}";

// ---------- Logging estruturado (Serilog) ----------
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Aplicacao", "PetHealthAPI")
    .WriteTo.Console(outputTemplate: TemplateLog)
    .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day, outputTemplate: TemplateLog)
    .WriteTo.File(new JsonFormatter(renderMessage: true), "logs/app-.json", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// User secrets em qualquer ambiente (inclusive ao rodar a DLL direto): a senha do Oracle nunca vai para o Git
builder.Configuration.AddUserSecrets<Program>(optional: true);

builder.Host.UseSerilog();

// ---------- Camadas (Clean Architecture) ----------
builder.Services.AddControllers()
    .AddJsonOptions(opcoes => opcoes.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping);
builder.Services.ConfigureHttpJsonOptions(opcoes =>
    opcoes.SerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping);
builder.Services.AddRouting(opcoes => opcoes.LowercaseUrls = true);
builder.Services.AdicionarSwagger();
builder.Services.AdicionarAplicacao();
builder.Services.AdicionarInfraestrutura(builder.Configuration);

// ---------- Segurança (JWT + perfis) ----------
builder.Services.AdicionarAutenticacaoJwt(builder.Configuration);

// ---------- Tratamento global de exceções ----------
builder.Services.AddExceptionHandler<ManipuladorExcecoesGlobal>();
builder.Services.AddProblemDetails();

// ---------- Observabilidade (OpenTelemetry) ----------
// Exportação no console configurável em "Observabilidade" (appsettings.json) para não poluir o terminal
var exportarTracesConsole = builder.Configuration.GetValue("Observabilidade:ExportarTracesNoConsole", true);
var exportarMetricasConsole = builder.Configuration.GetValue("Observabilidade:ExportarMetricasNoConsole", false);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(recurso => recurso.AddService(serviceName: "PetHealthAPI", serviceVersion: "4.0.0"))
    .WithTracing(tracing =>
    {
        tracing.AddSource(AplicacaoMetricas.NomeActivitySource)
               .AddAspNetCoreInstrumentation();

        if (exportarTracesConsole)
            tracing.AddConsoleExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics.AddMeter(AplicacaoMetricas.NomeServico)
               .AddAspNetCoreInstrumentation();

        if (exportarMetricasConsole)
            metrics.AddConsoleExporter();
    });

var app = builder.Build();

if (string.IsNullOrWhiteSpace(app.Configuration.GetConnectionString(InjecaoDependenciaInfraestrutura.NomeConnectionStringOracle)))
    Log.Warning("ConnectionStrings:OracleConnection não configurada. Configure com 'dotnet user-secrets' (veja o README).");

// ---------- Pipeline HTTP ----------
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseExceptionHandler();
app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI(opcoes =>
{
    opcoes.SwaggerEndpoint("/swagger/v1/swagger.json", "Pet Health API v1");
    opcoes.RoutePrefix = string.Empty;
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// /health: todos os checks (Oracle, MongoDB, memória) | /health/live: só indica que o processo está no ar
app.MapHealthChecks("/health", new HealthCheckOptions { ResponseWriter = RespostaHealthCheck.EscreverAsync });
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });

app.Run();

public partial class Program { }
