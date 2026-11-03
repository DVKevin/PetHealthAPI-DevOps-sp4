using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetHealthAPI.Dominio.Entidades;

namespace PetHealthAPI.Infraestrutura.Persistencia.Configuracoes;

public class PetConfiguracao : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("TB_PH_PET");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).HasColumnName("ID_PET");
        builder.Property(p => p.Nome).HasColumnName("NM_PET").HasMaxLength(100).IsRequired();
        builder.Property(p => p.Especie).HasColumnName("DS_ESPECIE").HasMaxLength(50).IsRequired();
        builder.Property(p => p.Raca).HasColumnName("DS_RACA").HasMaxLength(80);
        builder.Property(p => p.Idade).HasColumnName("NR_IDADE");
        builder.Property(p => p.Peso).HasColumnName("NR_PESO").HasPrecision(6, 2);
        builder.Property(p => p.Sexo).HasColumnName("DS_SEXO").HasMaxLength(10);
        builder.Property(p => p.Castrado).HasColumnName("FL_CASTRADO");
        builder.Property(p => p.Alergias).HasColumnName("DS_ALERGIAS").HasMaxLength(300);
        builder.Property(p => p.DataNascimento).HasColumnName("DT_NASCIMENTO");
        builder.Property(p => p.DataCadastro).HasColumnName("DT_CADASTRO");
        builder.Property(p => p.TutorId).HasColumnName("ID_TUTOR");

        builder.HasMany(p => p.Vacinas)
               .WithOne(v => v.Pet)
               .HasForeignKey(v => v.PetId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Consultas)
               .WithOne(c => c.Pet)
               .HasForeignKey(c => c.PetId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Medicamentos)
               .WithOne(m => m.Pet)
               .HasForeignKey(m => m.PetId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
