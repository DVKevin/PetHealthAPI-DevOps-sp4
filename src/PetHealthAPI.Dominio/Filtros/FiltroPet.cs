using PetHealthAPI.Dominio.Comum;

namespace PetHealthAPI.Dominio.Filtros;

/// <summary>Filtros de pets. Ordenação: nome, especie, idade, peso, dataCadastro.</summary>
public class FiltroPet : ParametrosConsulta
{
    /// <summary>Busca parcial pelo nome.</summary>
    public string? Nome { get; set; }

    /// <summary>Espécie exata (ex: Cão, Gato).</summary>
    public string? Especie { get; set; }

    /// <summary>Busca parcial pela raça.</summary>
    public string? Raca { get; set; }

    /// <summary>Somente castrados (true) ou não castrados (false).</summary>
    public bool? Castrado { get; set; }

    /// <summary>Pets de um tutor específico.</summary>
    public int? TutorId { get; set; }
}
