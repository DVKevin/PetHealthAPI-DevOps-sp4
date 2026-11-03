using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using PetHealthAPI.Dominio.Entidades;

namespace PetHealthAPI.Infraestrutura.Mongo;

/// <summary>
/// Mapeamento BSON feito na Infraestrutura (sem atributos do MongoDB na entidade de domínio).
/// </summary>
public static class MongoMapeamento
{
    private static readonly object Trava = new();

    public static void Registrar()
    {
        lock (Trava)
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(RegistroDiario)))
                return;

            BsonClassMap.RegisterClassMap<RegistroDiario>(mapa =>
            {
                mapa.MapIdMember(r => r.Id).SetSerializer(new StringSerializer(BsonType.ObjectId));
                mapa.MapMember(r => r.PetId).SetElementName("petId");
                mapa.MapMember(r => r.Data).SetElementName("data")
                    .SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));
                mapa.MapMember(r => r.Alimentacao).SetElementName("alimentacao");
                mapa.MapMember(r => r.Hidratacao).SetElementName("hidratacao");
                mapa.MapMember(r => r.Passeio).SetElementName("passeio");
                mapa.MapMember(r => r.MedicacaoEmDia).SetElementName("medicacaoEmDia");
                mapa.MapMember(r => r.Humor).SetElementName("humor");
                mapa.MapMember(r => r.Observacoes).SetElementName("observacoes");
                mapa.MapMember(r => r.Pontos).SetElementName("pontos");
                mapa.MapMember(r => r.DataRegistro).SetElementName("dataRegistro")
                    .SetSerializer(new DateTimeSerializer(DateTimeKind.Local));
                mapa.SetIgnoreExtraElements(true);
            });
        }
    }
}
