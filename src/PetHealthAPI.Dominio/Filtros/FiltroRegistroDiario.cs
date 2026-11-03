using PetHealthAPI.Dominio.Comum;

namespace PetHealthAPI.Dominio.Filtros;

/// <summary>Filtros do diário de cuidados (MongoDB). Ordenação: data, pontos.</summary>
public class FiltroRegistroDiario : ParametrosConsulta
{
    /// <summary>Registros de um pet específico.</summary>
    public int? PetId { get; set; }

    /// <summary>Registros a partir desta data.</summary>
    public DateTime? DataInicio { get; set; }

    /// <summary>Registros até esta data.</summary>
    public DateTime? DataFim { get; set; }
}
