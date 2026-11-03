using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PetHealthAPI.Infraestrutura.Seguranca;
using PetHealthAPI.Tratamento;

namespace PetHealthAPI.Configuracoes;

/// <summary>
/// Autenticação JWT Bearer + autorização por perfil (roles Admin e Usuario).
/// 401 e 403 seguem o mesmo formato ProblemDetails do restante da API.
/// </summary>
public static class AutenticacaoConfiguracao
{
    public const int TamanhoMinimoChave = 32;

    public static IServiceCollection AdicionarAutenticacaoJwt(this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwt = configuration.GetSection(JwtConfiguracao.Secao).Get<JwtConfiguracao>() ?? new JwtConfiguracao();

        if (string.IsNullOrWhiteSpace(jwt.Chave) || jwt.Chave.Length < TamanhoMinimoChave)
            throw new InvalidOperationException(
                $"Configure 'Jwt:Chave' com pelo menos {TamanhoMinimoChave} caracteres (appsettings.json ou user-secrets).");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opcoes =>
            {
                opcoes.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Emissor,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audiencia,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Chave)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1),
                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role
                };

                opcoes.Events = new JwtBearerEvents
                {
                    OnChallenge = async contexto =>
                    {
                        contexto.HandleResponse();
                        await EscritorProblemDetails.EscreverAsync(contexto.HttpContext,
                            StatusCodes.Status401Unauthorized, "Não autenticado",
                            "Envie um token JWT válido no header Authorization: Bearer {token}. " +
                            "Obtenha o token em POST /api/autenticacao/login.");
                    },
                    OnForbidden = contexto => EscritorProblemDetails.EscreverAsync(contexto.HttpContext,
                        StatusCodes.Status403Forbidden, "Acesso negado",
                        "Seu perfil não tem permissão para esta operação.")
                };
            });

        services.AddAuthorization();

        return services;
    }
}
