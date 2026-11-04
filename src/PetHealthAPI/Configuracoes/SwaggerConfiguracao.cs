using Microsoft.OpenApi.Models;

namespace PetHealthAPI.Configuracoes;

public static class SwaggerConfiguracao
{
    public const string EsquemaSeguranca = "Bearer";

    private static readonly string[] ArquivosXml =
    {
        "PetHealthAPI.xml",
        "PetHealthAPI.Aplicacao.xml",
        "PetHealthAPI.Dominio.xml"
    };

    public static IServiceCollection AdicionarSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opcoes =>
        {
            opcoes.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Pet Health API - Sprint 4 DevOps (deploy automático)",
                Version = "v1",
                Description = "API RESTful da plataforma Pet Health (Challenge FIAP 2026 — CLYVO VET). " +
                              "Tutores, pets, vacinas, consultas e medicamentos no Oracle; diário de cuidados " +
                              "gamificado no MongoDB. Listagens com paginação, filtros, ordenação e HATEOAS. " +
                              "Autenticação JWT: faça login em /api/autenticacao/login e clique em Authorize.",
                Contact = new OpenApiContact
                {
                    Name = "Pet Health — Challenge FIAP 2026",
                    Email = "rm556649@fiap.com.br"
                }
            });

            opcoes.AddSecurityDefinition(EsquemaSeguranca, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Cole apenas o token retornado pelo login (sem a palavra Bearer)."
            });

            opcoes.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = EsquemaSeguranca }
                    },
                    Array.Empty<string>()
                }
            });

            foreach (var arquivo in ArquivosXml)
            {
                var caminho = Path.Combine(AppContext.BaseDirectory, arquivo);
                if (File.Exists(caminho))
                    opcoes.IncludeXmlComments(caminho);
            }
        });

        return services;
    }
}
