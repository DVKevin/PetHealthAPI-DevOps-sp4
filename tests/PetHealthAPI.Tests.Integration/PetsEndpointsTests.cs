using System.Net;
using System.Net.Http.Json;
using PetHealthAPI.Tests.Integration.Infraestrutura;

namespace PetHealthAPI.Tests.Integration;

[Collection(IntegrationTestCollection.Nome)]
public class PetsEndpointsTests : IAsyncLifetime
{
    private readonly PetHealthApiFactory _factory;
    private HttpClient _client = null!;

    public PetsEndpointsTests(PetHealthApiFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = await _factory.CriarClienteAdminAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<int> CriarTutorAsync()
    {
        var resposta = await _client.PostAsJsonAsync("/api/tutores", new
        {
            nome = "Tutor do Pet",
            email = $"tutorpet{Guid.NewGuid():N}@teste.com",
            telefone = "11988887777",
            endereco = "Rua dos Pets, 10"
        });

        var corpo = await resposta.Content.ReadFromJsonAsync<RecursoResposta<TutorResposta>>();
        return corpo!.Dados.Id;
    }

    private static object NovoPet(int tutorId, string nome = "Rex") => new
    {
        nome,
        especie = "Cao",
        raca = "Vira-lata",
        idade = 2,
        peso = 10,
        sexo = "Macho",
        castrado = true,
        dataNascimento = "2024-01-01T00:00:00",
        tutorId
    };

    [Fact]
    public async Task PostPets_ComDadosValidos_RetornaCreated()
    {
        // Arrange
        var tutorId = await CriarTutorAsync();

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/pets", NovoPet(tutorId));

        // Assert
        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
    }

    [Fact]
    public async Task PostPets_ComNomeVazio_RetornaBadRequest()
    {
        // Arrange
        var tutorId = await CriarTutorAsync();

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/pets", NovoPet(tutorId, nome: ""));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task PostPets_ComTutorInexistente_RetornaBadRequest()
    {
        // Arrange
        const int tutorInexistente = 999999;

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/pets", NovoPet(tutorInexistente));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task GetPets_ComPaginacao_RetornaPaginaComLinksDeNavegacao()
    {
        // Arrange
        var tutorId = await CriarTutorAsync();
        for (var i = 0; i < 3; i++)
            await _client.PostAsJsonAsync("/api/pets", NovoPet(tutorId, $"Pet{i}"));

        // Act
        var resposta = await _client.GetAsync($"/api/pets?tutorId={tutorId}&pagina=1&tamanhoPagina=2&ordenarPor=nome");
        var pagina = await resposta.Content.ReadFromJsonAsync<PaginaResposta<PetResposta>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Equal(3, pagina!.TotalItens);
        Assert.Equal(2, pagina.Itens.Count);
        Assert.Equal("Pet0", pagina.Itens[0].Dados.Nome);
        Assert.Contains(pagina.Links, l => l.Rel == "proxima");
        Assert.All(pagina.Itens, item => Assert.Contains(item.Links, l => l.Rel == "vacinas"));
    }
}
