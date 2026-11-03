using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetHealthAPI.Dominio.Entidades;

namespace PetHealthAPI.Infraestrutura.Persistencia.Configuracoes;

public class ConsultaConfiguracao : IEntityTypeConfiguration<Consulta>
{
    public void Configure(EntityTypeBuilder<Consulta> builder)
    {
        builder.ToTable("TB_PH_CONSULTA");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).HasColumnName("ID_CONSULTA");
        builder.Property(c => c.DataConsulta).HasColumnName("DT_CONSULTA");
        builder.Property(c => c.Motivo).HasColumnName("DS_MOTIVO").HasMaxLength(200).IsRequired();
        builder.Property(c => c.Veterinario).HasColumnName("NM_VETERINARIO").HasMaxLength(150);
        builder.Property(c => c.Clinica).HasColumnName("NM_CLINICA").HasMaxLength(150);
        builder.Property(c => c.Diagnostico).HasColumnName("DS_DIAGNOSTICO").HasMaxLength(500);
        builder.Property(c => c.Tratamento).HasColumnName("DS_TRATAMENTO").HasMaxLength(500);
        builder.Property(c => c.Custo).HasColumnName("VL_CUSTO").HasPrecision(10, 2);
        builder.Property(c => c.DataRetorno).HasColumnName("DT_RETORNO");
        builder.Property(c => c.Observacoes).HasColumnName("DS_OBSERVACOES").HasMaxLength(300);
        builder.Property(c => c.PetId).HasColumnName("ID_PET");
    }
}
