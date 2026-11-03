using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using PetHealthAPI.Dominio.Comum;
using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Filtros;
using PetHealthAPI.Dominio.Interfaces;

namespace PetHealthAPI.Infraestrutura.Mongo;

/// <summary>
/// Repositório concreto NoSQL: CRUD, filtros, paginação e agregação ($match + $group) no MongoDB.
/// </summary>
public class RegistroDiarioRepositorioMongo : IRegistroDiarioRepositorio
{
    private readonly IMongoCollection<RegistroDiario> _colecao;

    public RegistroDiarioRepositorioMongo(IMongoDatabase banco, IOptions<MongoDbConfiguracao> configuracao)
    {
        _colecao = banco.GetCollection<RegistroDiario>(configuracao.Value.ColecaoRegistrosDiarios);
    }

    public async Task<RegistroDiario?> ObterPorIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return null;

        return await _colecao.Find(r => r.Id == id).FirstOrDefaultAsync();
    }

    public async Task<ResultadoPaginado<RegistroDiario>> ListarAsync(FiltroRegistroDiario filtro)
    {
        var construtor = Builders<RegistroDiario>.Filter;
        var condicao = construtor.Empty;

        if (filtro.PetId.HasValue)
            condicao &= construtor.Eq(r => r.PetId, filtro.PetId.Value);

        if (filtro.DataInicio.HasValue)
            condicao &= construtor.Gte(r => r.Data, RegistroDiario.NormalizarDia(filtro.DataInicio.Value));

        if (filtro.DataFim.HasValue)
            condicao &= construtor.Lte(r => r.Data, RegistroDiario.NormalizarDia(filtro.DataFim.Value));

        var totalItens = await _colecao.CountDocumentsAsync(condicao);

        var itens = await _colecao.Find(condicao)
            .Sort(CriarOrdenacao(filtro))
            .Skip((filtro.Pagina - 1) * filtro.TamanhoPagina)
            .Limit(filtro.TamanhoPagina)
            .ToListAsync();

        return new ResultadoPaginado<RegistroDiario>(itens, filtro.Pagina, filtro.TamanhoPagina, (int)totalItens);
    }

    public async Task AdicionarAsync(RegistroDiario entidade)
    {
        entidade.DefinirId(ObjectId.GenerateNewId().ToString());
        await _colecao.InsertOneAsync(entidade);
    }

    public async Task AtualizarAsync(RegistroDiario entidade)
    {
        await _colecao.ReplaceOneAsync(r => r.Id == entidade.Id, entidade);
    }

    public async Task RemoverAsync(RegistroDiario entidade)
    {
        await _colecao.DeleteOneAsync(r => r.Id == entidade.Id);
    }

    public async Task<int> SomarPontosDoPetAsync(int petId)
    {
        var resultado = await _colecao.Aggregate()
            .Match(r => r.PetId == petId)
            .Group(r => r.PetId, grupo => new { PetId = grupo.Key, Total = grupo.Sum(r => r.Pontos) })
            .FirstOrDefaultAsync();

        return resultado?.Total ?? 0;
    }

    private static SortDefinition<RegistroDiario> CriarOrdenacao(FiltroRegistroDiario filtro)
    {
        var construtor = Builders<RegistroDiario>.Sort;
        var descendente = filtro.EhDescendente();

        return filtro.OrdenarPor?.Trim().ToLowerInvariant() switch
        {
            "pontos" => descendente ? construtor.Descending(r => r.Pontos) : construtor.Ascending(r => r.Pontos),
            _ => descendente ? construtor.Descending(r => r.Data) : construtor.Ascending(r => r.Data)
        };
    }
}
