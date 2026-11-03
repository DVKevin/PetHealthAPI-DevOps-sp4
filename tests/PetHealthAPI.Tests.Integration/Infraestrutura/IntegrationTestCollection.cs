namespace PetHealthAPI.Tests.Integration.Infraestrutura;

/// <summary>
/// Collection Fixture: todas as classes de teste de integração compartilham a mesma API em memória.
/// </summary>
[CollectionDefinition(Nome)]
public class IntegrationTestCollection : ICollectionFixture<PetHealthApiFactory>
{
    public const string Nome = "Integration Tests";
}
