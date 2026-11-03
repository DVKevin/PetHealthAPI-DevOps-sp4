using PetHealthAPI.Aplicacao.Interfaces;
using PetHealthAPI.Aplicacao.Servicos;

namespace PetHealthAPI.Configuracoes;

/// <summary>
/// Registro dos serviços da camada de Aplicação (feito na API, que é a raiz de composição,
/// para que a Aplicação não dependa de nenhum pacote de framework).
/// </summary>
public static class InjecaoDependenciaAplicacao
{
    public static IServiceCollection AdicionarAplicacao(this IServiceCollection services)
    {
        services.AddScoped<ITutorServico, TutorServico>();
        services.AddScoped<IPetServico, PetServico>();
        services.AddScoped<IVacinaServico, VacinaServico>();
        services.AddScoped<IConsultaServico, ConsultaServico>();
        services.AddScoped<IMedicamentoServico, MedicamentoServico>();
        services.AddScoped<IRegistroDiarioServico, RegistroDiarioServico>();
        services.AddScoped<IAutenticacaoServico, AutenticacaoServico>();

        return services;
    }
}
