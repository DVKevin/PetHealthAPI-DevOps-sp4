using System.Collections.Concurrent;
using PetHealthAPI.Dominio.Comum;
using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Filtros;
using PetHealthAPI.Dominio.Interfaces;

namespace PetHealthAPI.Tests.Integration.Infraestrutura;

/// <summary>Substituto do repositório MongoDB para os testes de integração.</summary>
public class RegistroDiarioRepositorioEmMemoria : IRegistroDiarioRepositorio
{
    private readonly ConcurrentDictionary<string, RegistroDiario> _registros = new();

    public Task<RegistroDiario?> ObterPorIdAsync(string id)
    {
        _registros.TryGetValue(id, out var registro);
        return Task.FromResult(registro);
    }

    public Task<ResultadoPaginado<RegistroDiario>> ListarAsync(FiltroRegistroDiario filtro)
    {
        var consulta = _registros.Values.AsEnumerable();

        if (filtro.PetId.HasValue)
            consulta = consulta.Where(r => r.PetId == filtro.PetId.Value);

        var todos = consulta.OrderBy(r => r.Data).ToList();
        var itens = todos.Skip((filtro.Pagina - 1) * filtro.TamanhoPagina).Take(filtro.TamanhoPagina).ToList();

        return Task.FromResult(new ResultadoPaginado<RegistroDiario>(itens, filtro.Pagina, filtro.TamanhoPagina, todos.Count));
    }

    public Task AdicionarAsync(RegistroDiario entidade)
    {
        // Mesmo formato de um ObjectId (24 caracteres hexadecimais)
        entidade.DefinirId(Guid.NewGuid().ToString("N")[..24]);
        _registros[entidade.Id] = entidade;
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(RegistroDiario entidade)
    {
        _registros[entidade.Id] = entidade;
        return Task.CompletedTask;
    }

    public Task RemoverAsync(RegistroDiario entidade)
    {
        _registros.TryRemove(entidade.Id, out _);
        return Task.CompletedTask;
    }

    public Task<int> SomarPontosDoPetAsync(int petId)
    {
        return Task.FromResult(_registros.Values.Where(r => r.PetId == petId).Sum(r => r.Pontos));
    }
}
