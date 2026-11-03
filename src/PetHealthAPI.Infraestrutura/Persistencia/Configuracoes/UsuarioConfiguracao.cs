using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetHealthAPI.Dominio.Entidades;

namespace PetHealthAPI.Infraestrutura.Persistencia.Configuracoes;

public class UsuarioConfiguracao : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("TB_PH_USUARIO");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id).HasColumnName("ID_USUARIO");
        builder.Property(u => u.Nome).HasColumnName("NM_USUARIO").HasMaxLength(100).IsRequired();
        builder.Property(u => u.Email).HasColumnName("DS_EMAIL").HasMaxLength(150).IsRequired();
        builder.Property(u => u.SenhaHash).HasColumnName("DS_SENHA_HASH").HasMaxLength(500).IsRequired();
        builder.Property(u => u.Perfil).HasColumnName("DS_PERFIL").HasMaxLength(20).IsRequired();
        builder.Property(u => u.DataCadastro).HasColumnName("DT_CADASTRO");

        builder.HasIndex(u => u.Email).IsUnique();
    }
}
