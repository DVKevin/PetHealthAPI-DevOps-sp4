using PetHealthAPI.Dominio.Comum;

namespace PetHealthAPI.Dominio.Filtros;

/// <summary>Filtros de medicamentos. Ordenação: nome, dataInicio, dataFim.</summary>
public class FiltroMedicamento : ParametrosConsulta
{
    /// <summary>Medicamentos de um pet específico.</summary>
    public int? PetId { get; set; }

    /// <summary>Busca parcial pelo nome do medicamento.</summary>
    public string? Nome { get; set; }

    /// <summary>Somente ativos (true) ou encerrados (false).</summary>
    public bool? Ativo { get; set; }
}
