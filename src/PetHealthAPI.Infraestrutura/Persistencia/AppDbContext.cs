using Microsoft.EntityFrameworkCore;
using PetHealthAPI.Dominio.Entidades;

namespace PetHealthAPI.Infraestrutura.Persistencia;

/// <summary>
/// Contexto relacional (Oracle). Os mapeamentos ficam em classes IEntityTypeConfiguration
/// para manter as entidades de domínio livres de atributos de banco.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Tutor> Tutores => Set<Tutor>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<Vacina> Vacinas => Set<Vacina>();
    public DbSet<Consulta> Consultas => Set<Consulta>();
    public DbSet<Medicamento> Medicamentos => Set<Medicamento>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
