using System.Net;
using PetHealthAPI.Tests.Integration.Infraestrutura;

namespace PetHealthAPI.Tests.Integration;

[Collection(IntegrationTestCollection.Nome)]
public class HealthEndpointsTests
{
    private readonly HttpClient _client;

    public HealthEndpointsTests(PetHealthApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealth_QuandoChamado_RetornaOk()
    {
        // Arrange
        // (nenhuma preparação adicional necessária)

        // Act
        var resposta = await _client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
    }

    [Fact]
    public async Task GetHealthLive_QuandoChamado_RetornaOk()
    {
        // Arrange
        // (nenhuma preparação adicional necessária)

        // Act
        var resposta = await _client.GetAsync("/health/live");

        // Assert
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
    }
}
