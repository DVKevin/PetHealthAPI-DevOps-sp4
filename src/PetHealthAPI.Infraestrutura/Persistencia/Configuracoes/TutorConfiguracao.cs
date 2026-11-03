using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetHealthAPI.Dominio.Entidades;

namespace PetHealthAPI.Infraestrutura.Persistencia.Configuracoes;

public class TutorConfiguracao : IEntityTypeConfiguration<Tutor>
{
    public void Configure(EntityTypeBuilder<Tutor> builder)
    {
        builder.ToTable("TB_PH_TUTOR");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id).HasColumnName("ID_TUTOR");
        builder.Property(t => t.Nome).HasColumnName("NM_TUTOR").HasMaxLength(100).IsRequired();
        builder.Property(t => t.Email).HasColumnName("DS_EMAIL").HasMaxLength(150).IsRequired();
        builder.Property(t => t.Telefone).HasColumnName("NR_TELEFONE").HasMaxLength(20).IsRequired();
        builder.Property(t => t.Endereco).HasColumnName("DS_ENDERECO").HasMaxLength(200);
        builder.Property(t => t.DataCadastro).HasColumnName("DT_CADASTRO");

        builder.HasIndex(t => t.Email).IsUnique();

        builder.HasMany(t => t.Pets)
               .WithOne(p => p.Tutor)
               .HasForeignKey(p => p.TutorId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
