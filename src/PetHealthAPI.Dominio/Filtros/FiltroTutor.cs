using PetHealthAPI.Dominio.Comum;

namespace PetHealthAPI.Dominio.Filtros;

/// <summary>Filtros de tutores. Ordenação: nome, email, dataCadastro.</summary>
public class FiltroTutor : ParametrosConsulta
{
    /// <summary>Busca parcial pelo nome.</summary>
    public string? Nome { get; set; }

    /// <summary>Busca parcial pelo email.</summary>
    public string? Email { get; set; }
}
