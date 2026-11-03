using PetHealthAPI.Dominio.Comum;

namespace PetHealthAPI.Dominio.Filtros;

/// <summary>Filtros de consultas. Ordenação: dataConsulta, custo, dataRetorno.</summary>
public class FiltroConsulta : ParametrosConsulta
{
    /// <summary>Consultas de um pet específico.</summary>
    public int? PetId { get; set; }

    /// <summary>Busca parcial pelo nome do veterinário.</summary>
    public string? Veterinario { get; set; }

    /// <summary>Consultas a partir desta data.</summary>
    public DateTime? DataInicio { get; set; }

    /// <summary>Consultas até esta data.</summary>
    public DateTime? DataFim { get; set; }

    /// <summary>Somente consultas com retorno agendado a partir de hoje.</summary>
    public bool? ComRetornoPendente { get; set; }
}
