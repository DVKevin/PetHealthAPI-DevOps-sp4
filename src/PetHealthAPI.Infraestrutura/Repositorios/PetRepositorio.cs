using Microsoft.EntityFrameworkCore;
using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Filtros;
using PetHealthAPI.Dominio.Interfaces;
using PetHealthAPI.Infraestrutura.Persistencia;
using PetHealthAPI.Infraestrutura.Persistencia.Extensoes;

namespace PetHealthAPI.Infraestrutura.Repositorios;

public class PetRepositorio : RepositorioBase<Pet, FiltroPet>, IPetRepositorio
{
    public PetRepositorio(AppDbContext contexto) : base(contexto) { }

    public override async Task<Pet?> ObterPorIdAsync(int id)
    {
        return await Contexto.Pets
            .Include(p => p.Tutor)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> ExisteAsync(int petId)
    {
        return await Contexto.Pets.AnyAsync(p => p.Id == petId);
    }

    protected override IQueryable<Pet> AplicarFiltros(IQueryable<Pet> consulta, FiltroPet filtro)
    {
        consulta = consulta.Include(p => p.Tutor);

        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            var nome = filtro.Nome.Trim().ToLowerInvariant();
            consulta = consulta.Where(p => p.Nome.ToLower().Contains(nome));
        }

        if (!string.IsNullOrWhiteSpace(filtro.Especie))
        {
            var especie = filtro.Especie.Trim().ToLowerInvariant();
            consulta = consulta.Where(p => p.Especie.ToLower() == especie);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Raca))
        {
            var raca = filtro.Raca.Trim().ToLowerInvariant();
            consulta = consulta.Where(p => p.Raca != null && p.Raca.ToLower().Contains(raca));
        }

        if (filtro.Castrado.HasValue)
            consulta = consulta.Where(p => p.Castrado == filtro.Castrado.Value);

        if (filtro.TutorId.HasValue)
            consulta = consulta.Where(p => p.TutorId == filtro.TutorId.Value);

        return consulta;
    }

    protected override IQueryable<Pet> AplicarOrdenacao(IQueryable<Pet> consulta, FiltroPet filtro)
    {
        var descendente = filtro.EhDescendente();

        return filtro.OrdenarPor?.Trim().ToLower() switch
        {
            "nome" => consulta.Ordenar(p => p.Nome, descendente),
            "especie" => consulta.Ordenar(p => p.Especie, descendente),
            "idade" => consulta.Ordenar(p => p.Idade, descendente),
            "peso" => consulta.Ordenar(p => p.Peso, descendente),
            "datacadastro" => consulta.Ordenar(p => p.DataCadastro, descendente),
            _ => consulta.Ordenar(p => p.Id, descendente)
        };
    }
}
