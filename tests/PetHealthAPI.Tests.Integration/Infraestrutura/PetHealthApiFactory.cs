using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PetHealthAPI.Dominio.Interfaces;
using PetHealthAPI.Infraestrutura;
using PetHealthAPI.Infraestrutura.Persistencia;

namespace PetHealthAPI.Tests.Integration.Infraestrutura;

/// <summary>
/// Sobe a API real em memória, trocando só o que é externo:
/// Oracle → EF Core InMemory e MongoDB → repositório em memória.
/// Assim os testes não dependem de banco, rede ou dados pré-existentes.
/// </summary>
public class PetHealthApiFactory : WebApplicationFactory<Program>
{
    private readonly string _nomeBanco = $"PetHealthTestes-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Oracle → InMemory
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(_nomeBanco));

            // MongoDB → repositório em memória
            services.RemoveAll<IRegistroDiarioRepositorio>();
            services.AddSingleton<IRegistroDiarioRepositorio, RegistroDiarioRepositorioEmMemoria>();

            // O health check do MongoDB não faz sentido sem o container rodando
            services.Configure<HealthCheckServiceOptions>(opcoes =>
            {
                var checkMongo = opcoes.Registrations
                    .FirstOrDefault(r => r.Name == InjecaoDependenciaInfraestrutura.NomeHealthCheckMongo);

                if (checkMongo is not null)
                    opcoes.Registrations.Remove(checkMongo);
            });
        });
    }

    /// <summary>Cliente autenticado como o Admin criado automaticamente na inicialização.</summary>
    public Task<HttpClient> CriarClienteAdminAsync()
    {
        return CriarClienteAutenticadoAsync(CredenciaisTeste.EmailAdmin, CredenciaisTeste.SenhaAdmin);
    }

    /// <summary>Cadastra um usuário novo (perfil Usuario) e devolve um cliente autenticado com ele.</summary>
    public async Task<HttpClient> CriarClienteUsuarioAsync()
    {
        var email = $"usuario{Guid.NewGuid():N}@teste.com";
        var cliente = CreateClient();

        var cadastro = await cliente.PostAsJsonAsync("/api/autenticacao/registrar",
            new { nome = "Usuario Teste", email, senha = CredenciaisTeste.SenhaUsuario });
        cadastro.EnsureSuccessStatusCode();

        return await CriarClienteAutenticadoAsync(email, CredenciaisTeste.SenhaUsuario);
    }

    public async Task<HttpClient> CriarClienteAutenticadoAsync(string email, string senha)
    {
        var cliente = CreateClient();

        var resposta = await cliente.PostAsJsonAsync("/api/autenticacao/login", new { email, senha });
        resposta.EnsureSuccessStatusCode();

        var token = await resposta.Content.ReadFromJsonAsync<TokenResposta>();
        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token!.Token);

        return cliente;
    }
}
