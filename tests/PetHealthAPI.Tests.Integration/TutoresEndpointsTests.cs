using System.Net;
using System.Net.Http.Json;
using PetHealthAPI.Tests.Integration.Infraestrutura;

namespace PetHealthAPI.Tests.Integration;

[Collection(IntegrationTestCollection.Nome)]
public class TutoresEndpointsTests : IAsyncLifetime
{
    private readonly PetHealthApiFactory _factory;
    private HttpClient _client = null!;

    public TutoresEndpointsTests(PetHealthApiFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = await _factory.CriarClienteAdminAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private static object NovoTutor(string? email = null) => new
    {
        nome = "Tutor Teste",
        email = email ?? $"tutor{Guid.NewGuid():N}@teste.com",
        telefone = "11999999999",
        endereco = "Rua Teste, 123"
    };

    [Fact]
    public async Task PostTutores_ComDadosValidos_RetornaCreatedComLinks()
    {
        // Arrange
        var novoTutor = NovoTutor();

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/tutores", novoTutor);
        var corpo = await resposta.Content.ReadFromJsonAsync<RecursoResposta<TutorResposta>>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        Assert.NotNull(corpo);
        Assert.Contains(corpo!.Links, l => l.Rel == "self" && l.Metodo == "GET");
    }

    [Fact]
    public async Task PostTutores_ComEmailInvalido_RetornaBadRequest()
    {
        // Arrange
        var tutorInvalido = NovoTutor(email: "nao-e-um-email");

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/tutores", tutorInvalido);
        var problema = await resposta.Content.ReadFromJsonAsync<ProblemaResposta>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        Assert.Equal("O email do tutor é inválido.", problema!.Detail);
    }

    [Fact]
    public async Task PostTutores_ComEmailDuplicado_RetornaConflict()
    {
        // Arrange
        var email = $"duplicado{Guid.NewGuid():N}@teste.com";
        await _client.PostAsJsonAsync("/api/tutores", NovoTutor(email));

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/tutores", NovoTutor(email));

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, resposta.StatusCode);
    }

    [Fact]
    public async Task GetTutor_Inexistente_RetornaNotFoundComProblemDetails()
    {
        // Arrange
        const int idInexistente = 999999;

        // Act
        var resposta = await _client.GetAsync($"/api/tutores/{idInexistente}");
        var problema = await resposta.Content.ReadFromJsonAsync<ProblemaResposta>();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        Assert.Equal($"Tutor com ID {idInexistente} não encontrado.", problema!.Detail);
    }
}
