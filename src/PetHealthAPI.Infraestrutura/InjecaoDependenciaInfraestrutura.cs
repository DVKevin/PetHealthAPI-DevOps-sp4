using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PetHealthAPI.Aplicacao.Interfaces;
using PetHealthAPI.Dominio.Interfaces;
using PetHealthAPI.Infraestrutura.Health;
using PetHealthAPI.Infraestrutura.Mongo;
using PetHealthAPI.Infraestrutura.Observabilidade;
using PetHealthAPI.Infraestrutura.Persistencia;
using PetHealthAPI.Infraestrutura.Repositorios;
using PetHealthAPI.Infraestrutura.Seguranca;

namespace PetHealthAPI.Infraestrutura;

/// <summary>
/// Registro de todas as dependências da camada de Infraestrutura (Oracle, MongoDB, segurança, health checks e métricas).
/// </summary>
public static class InjecaoDependenciaInfraestrutura
{
    public const string NomeConnectionStringOracle = "OracleConnection";
    public const string TabelaHistoricoMigrations = "__EFMigrationsHistory_PetHealth";
    public const string NomeHealthCheckOracle = "oracle";
    public const string NomeHealthCheckMongo = "mongodb";
    public const string NomeHealthCheckMemoria = "memoria";

    public static IServiceCollection AdicionarInfraestrutura(this IServiceCollection services,
        IConfiguration configuration)
    {
        // ---------- Oracle (EF Core) ----------
        services.AddDbContext<AppDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString(NomeConnectionStringOracle);

            // Sem connection string a API ainda sobe (Swagger e /health funcionam e mostram o problema).
            if (string.IsNullOrWhiteSpace(connectionString))
                connectionString = "Data Source=connection-string-nao-configurada";

            options.UseOracle(connectionString, oracle =>
                oracle.MigrationsHistoryTable(TabelaHistoricoMigrations));
        });

        services.AddScoped<ITutorRepositorio, TutorRepositorio>();
        services.AddScoped<IPetRepositorio, PetRepositorio>();
        services.AddScoped<IVacinaRepositorio, VacinaRepositorio>();
        services.AddScoped<IConsultaRepositorio, ConsultaRepositorio>();
        services.AddScoped<IMedicamentoRepositorio, MedicamentoRepositorio>();
        services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

        // ---------- MongoDB ----------
        MongoMapeamento.Registrar();

        services.Configure<MongoDbConfiguracao>(configuration.GetSection(MongoDbConfiguracao.Secao));

        services.AddSingleton<IMongoClient>(provider =>
        {
            var mongo = provider.GetRequiredService<IOptions<MongoDbConfiguracao>>().Value;
            var configuracaoCliente = MongoClientSettings.FromConnectionString(mongo.ConnectionString);
            configuracaoCliente.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
            return new MongoClient(configuracaoCliente);
        });

        services.AddSingleton(provider =>
        {
            var mongo = provider.GetRequiredService<IOptions<MongoDbConfiguracao>>().Value;
            return provider.GetRequiredService<IMongoClient>().GetDatabase(mongo.NomeBanco);
        });

        services.AddScoped<IRegistroDiarioRepositorio, RegistroDiarioRepositorioMongo>();

        // ---------- Segurança (JWT e senhas) ----------
        services.Configure<JwtConfiguracao>(configuration.GetSection(JwtConfiguracao.Secao));
        services.Configure<AdminPadraoConfiguracao>(configuration.GetSection(AdminPadraoConfiguracao.Secao));
        services.AddSingleton<ISenhaHasher, SenhaHasher>();
        services.AddSingleton<IGeradorToken, GeradorTokenJwt>();
        services.AddHostedService<InicializadorAdminPadrao>();

        // ---------- Observabilidade ----------
        services.AddSingleton<AplicacaoMetricas>();

        // ---------- Health Checks ----------
        services.AddHealthChecks()
            .AddCheck<OracleHealthCheck>(NomeHealthCheckOracle, HealthStatus.Unhealthy, new[] { "db", "oracle" })
            .AddCheck<MongoDbHealthCheck>(NomeHealthCheckMongo, HealthStatus.Degraded, new[] { "db", "nosql" })
            .AddCheck<MemoriaHealthCheck>(NomeHealthCheckMemoria, HealthStatus.Degraded, new[] { "infra" });

        return services;
    }
}
