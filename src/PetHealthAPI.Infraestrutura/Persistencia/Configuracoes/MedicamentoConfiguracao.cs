using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetHealthAPI.Dominio.Entidades;

namespace PetHealthAPI.Infraestrutura.Persistencia.Configuracoes;

public class MedicamentoConfiguracao : IEntityTypeConfiguration<Medicamento>
{
    public void Configure(EntityTypeBuilder<Medicamento> builder)
    {
        builder.ToTable("TB_PH_MEDICAMENTO");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id).HasColumnName("ID_MEDICAMENTO");
        builder.Property(m => m.Nome).HasColumnName("NM_MEDICAMENTO").HasMaxLength(150).IsRequired();
        builder.Property(m => m.Dosagem).HasColumnName("DS_DOSAGEM").HasMaxLength(100).IsRequired();
        builder.Property(m => m.Frequencia).HasColumnName("DS_FREQUENCIA").HasMaxLength(100).IsRequired();
        builder.Property(m => m.DataInicio).HasColumnName("DT_INICIO");
        builder.Property(m => m.DataFim).HasColumnName("DT_FIM");
        builder.Property(m => m.VeterinarioPrescreveu).HasColumnName("NM_VETERINARIO_PRESCREVEU").HasMaxLength(150);
        builder.Property(m => m.Ativo).HasColumnName("FL_ATIVO");
        builder.Property(m => m.Observacoes).HasColumnName("DS_OBSERVACOES").HasMaxLength(300);
        builder.Property(m => m.PetId).HasColumnName("ID_PET");
    }
}
