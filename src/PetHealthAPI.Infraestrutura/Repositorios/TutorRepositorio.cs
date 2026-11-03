using Microsoft.EntityFrameworkCore;
using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Filtros;
using PetHealthAPI.Dominio.Interfaces;
using PetHealthAPI.Infraestrutura.Persistencia;
using PetHealthAPI.Infraestrutura.Persistencia.Extensoes;

namespace PetHealthAPI.Infraestrutura.Repositorios;

public class TutorRepositorio : RepositorioBase<Tutor, FiltroTutor>, ITutorRepositorio
{
    public TutorRepositorio(AppDbContext contexto) : base(contexto) { }

    public async Task<bool> ExisteAsync(int tutorId)
    {
        return await Contexto.Tutores.AnyAsync(t => t.Id == tutorId);
    }

    public async Task<bool> EmailEmUsoAsync(string email, int? ignorarId)
    {
        var emailNormalizado = email.Trim().ToLowerInvariant();
        return await Contexto.Tutores.AnyAsync(t => t.Email == emailNormalizado &&
                                                    (!ignorarId.HasValue || t.Id != ignorarId.Value));
    }

    protected override IQueryable<Tutor> AplicarFiltros(IQueryable<Tutor> consulta, FiltroTutor filtro)
    {
        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            var nome = filtro.Nome.Trim().ToLowerInvariant();
            consulta = consulta.Where(t => t.Nome.ToLower().Contains(nome));
        }

        if (!string.IsNullOrWhiteSpace(filtro.Email))
        {
            var email = filtro.Email.Trim().ToLowerInvariant();
            consulta = consulta.Where(t => t.Email.Contains(email));
        }

        return consulta;
    }

    protected override IQueryable<Tutor> AplicarOrdenacao(IQueryable<Tutor> consulta, FiltroTutor filtro)
    {
        var descendente = filtro.EhDescendente();

        return filtro.OrdenarPor?.Trim().ToLower() switch
        {
            "nome" => consulta.Ordenar(t => t.Nome, descendente),
            "email" => consulta.Ordenar(t => t.Email, descendente),
            "datacadastro" => consulta.Ordenar(t => t.DataCadastro, descendente),
            _ => consulta.Ordenar(t => t.Id, descendente)
        };
    }
}
