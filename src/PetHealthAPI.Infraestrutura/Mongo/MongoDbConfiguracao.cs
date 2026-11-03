namespace PetHealthAPI.Infraestrutura.Mongo;

/// <summary>Seção "MongoDb" do appsettings.json.</summary>
public class MongoDbConfiguracao
{
    public const string Secao = "MongoDb";

    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string NomeBanco { get; set; } = "pethealth";
    public string ColecaoRegistrosDiarios { get; set; } = "registros_diarios";
}
