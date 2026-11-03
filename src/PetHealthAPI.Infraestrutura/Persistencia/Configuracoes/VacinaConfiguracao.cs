using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetHealthAPI.Dominio.Entidades;

namespace PetHealthAPI.Infraestrutura.Persistencia.Configuracoes;

public class VacinaConfiguracao : IEntityTypeConfiguration<Vacina>
{
    public void Configure(EntityTypeBuilder<Vacina> builder)
    {
        builder.ToTable("TB_PH_VACINA");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id).HasColumnName("ID_VACINA");
        builder.Property(v => v.Nome).HasColumnName("NM_VACINA").HasMaxLength(150).IsRequired();
        builder.Property(v => v.DataAplicacao).HasColumnName("DT_APLICACAO");
        builder.Property(v => v.DataProximaDose).HasColumnName("DT_PROXIMA_DOSE");
        builder.Property(v => v.Fabricante).HasColumnName("NM_FABRICANTE").HasMaxLength(100);
        builder.Property(v => v.Lote).HasColumnName("NR_LOTE").HasMaxLength(80);
        builder.Property(v => v.Veterinario).HasColumnName("NM_VETERINARIO").HasMaxLength(150);
        builder.Property(v => v.Observacoes).HasColumnName("DS_OBSERVACOES").HasMaxLength(300);
        builder.Property(v => v.PetId).HasColumnName("ID_PET");
    }
}
