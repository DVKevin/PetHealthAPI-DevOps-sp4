using PetHealthAPI.Dominio.Entidades;

namespace PetHealthAPI.Aplicacao.Interfaces;

public record TokenGerado(string Token, DateTime ExpiraEm);

/// <summary>Abstração da emissão de JWT (implementada na Infraestrutura).</summary>
public interface IGeradorToken
{
    TokenGerado Gerar(Usuario usuario);
}
