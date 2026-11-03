using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetHealthAPI.Aplicacao.Interfaces;
using PetHealthAPI.Dominio.Constantes;
using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Interfaces;

namespace PetHealthAPI.Infraestrutura.Seguranca;

/// <summary>
/// Na inicialização da API, garante que exista um usuário Admin (o cadastro público só cria perfil "Usuario").
/// Se o banco estiver indisponível, registra um aviso e a API continua subindo.
/// </summary>
public class InicializadorAdminPadrao : IHostedService
{
    private readonly IServiceScopeFactory _fabricaEscopo;
    private readonly AdminPadraoConfiguracao _configuracao;
    private readonly ILogger<InicializadorAdminPadrao> _logger;

    public InicializadorAdminPadrao(IServiceScopeFactory fabricaEscopo, IOptions<AdminPadraoConfiguracao> configuracao,
                                    ILogger<InicializadorAdminPadrao> logger)
    {
        _fabricaEscopo = fabricaEscopo;
        _configuracao = configuracao.Value;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_configuracao.Email) || string.IsNullOrWhiteSpace(_configuracao.Senha))
        {
            _logger.LogWarning("AdminPadrao não configurado: nenhum administrador será criado.");
            return;
        }

        try
        {
            using var escopo = _fabricaEscopo.CreateScope();
            var usuarioRepositorio = escopo.ServiceProvider.GetRequiredService<IUsuarioRepositorio>();
            var senhaHasher = escopo.ServiceProvider.GetRequiredService<ISenhaHasher>();

            if (await usuarioRepositorio.EmailEmUsoAsync(_configuracao.Email))
                return;

            var admin = new Usuario(_configuracao.Nome, _configuracao.Email,
                                    senhaHasher.GerarHash(_configuracao.Senha), PerfisUsuario.Admin);

            await usuarioRepositorio.AdicionarAsync(admin);
            _logger.LogInformation("Administrador padrão criado: {Email}", admin.Email);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Não foi possível garantir o administrador padrão (banco indisponível ou migration não aplicada).");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
