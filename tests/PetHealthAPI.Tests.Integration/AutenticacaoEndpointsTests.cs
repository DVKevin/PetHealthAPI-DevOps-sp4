using System.Net;
using System.Net.Http.Json;
using PetHealthAPI.Tests.Integration.Infraestrutura;

namespace PetHealthAPI.Tests.Integration;

[Collection(IntegrationTestCollection.Nome)]
public class AutenticacaoEndpointsTests
{
    private readonly PetHealthApiFactory _factory;

    public AutenticacaoEndpointsTests(PetHealthApiFactory factory)
    {
        _factory = factory;
    }

    private async Task<int> CriarTutorComoAdminAsync()
    {
        var admin = await _factory.CriarClienteAdminAsync();
        var resposta = await admin.PostAsJsonAsync("/api/tutores", new
        {
            nome = "Tutor Autorizacao",
            email = $"auth{Guid.NewGuid():N}@teste.com",
            telefone = "11977776666",
            endereco = (string?)null
        });

        var corpo = await resposta.Content.ReadFromJsonAsync<RecursoResposta<TutorResposta>>();
        return corpo!.Dados.Id;
    }

    [Fact]
    public async Task PostLogin_CredenciaisDoAdmin_RetornaTokenComPerfilAdmin()
    {
        // Arrange
        var cliente = _factory.CreateClient();
        var credenciais = new { email = CredenciaisTeste.EmailAdmin, senha = CredenciaisTeste.SenhaAdmin };

        // Act
        var resposta = await cliente.PostAsJsonAsync("/api/autenticacao/login", credenciais);
        var token = await resposta.Content.ReadFromJsonAsync<TokenResposta>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(token!.Token));
        Assert.Equal("Admin", token.Perfil);
    }

    [Fact]
    public async Task PostLogin_SenhaIncorreta_RetornaUnauthorized()
    {
        // Arrange
        var cliente = _factory.CreateClient();
        var credenciais = new { email = CredenciaisTeste.EmailAdmin, senha = "senha-errada" };

        // Act
        var resposta = await cliente.PostAsJsonAsync("/api/autenticacao/login", credenciais);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task GetPets_SemToken_RetornaUnauthorizedComProblemDetails()
    {
        // Arrange
        var clienteAnonimo = _factory.CreateClient();

        // Act
        var resposta = await clienteAnonimo.GetAsync("/api/pets");
        var problema = await resposta.Content.ReadFromJsonAsync<ProblemaResposta>();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
        Assert.Equal("Não autenticado", problema!.Title);
    }

    [Fact]
    public async Task PostRegistrar_EmailJaCadastrado_RetornaConflict()
    {
        // Arrange
        var cliente = _factory.CreateClient();
        var usuario = new { nome = "Repetido", email = $"repetido{Guid.NewGuid():N}@teste.com", senha = "Senha@123" };
        await cliente.PostAsJsonAsync("/api/autenticacao/registrar", usuario);

        // Act
        var resposta = await cliente.PostAsJsonAsync("/api/autenticacao/registrar", usuario);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, resposta.StatusCode);
    }

    [Fact]
    public async Task DeleteTutor_ComPerfilUsuario_RetornaForbidden()
    {
        // Arrange
        var tutorId = await CriarTutorComoAdminAsync();
        var usuarioComum = await _factory.CriarClienteUsuarioAsync();

        // Act
        var resposta = await usuarioComum.DeleteAsync($"/api/tutores/{tutorId}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }

    [Fact]
    public async Task DeleteTutor_ComPerfilAdmin_RetornaNoContent()
    {
        // Arrange
        var tutorId = await CriarTutorComoAdminAsync();
        var admin = await _factory.CriarClienteAdminAsync();

        // Act
        var resposta = await admin.DeleteAsync($"/api/tutores/{tutorId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, resposta.StatusCode);
    }
}
