using PetHealthAPI.Dominio.Comum;

namespace PetHealthAPI.Dominio.Filtros;

/// <summary>Filtros de vacinas. Ordenação: nome, dataAplicacao, dataProximaDose.</summary>
public class FiltroVacina : ParametrosConsulta
{
    /// <summary>Vacinas de um pet específico.</summary>
    public int? PetId { get; set; }

    /// <summary>Busca parcial pelo nome da vacina.</summary>
    public string? Nome { get; set; }

    /// <summary>Somente vacinas com próxima dose nos próximos N dias.</summary>
    public int? VencendoEmDias { get; set; }
}
