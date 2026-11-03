namespace PetHealthAPI.Hateoas;

/// <summary>Link HATEOAS: para onde o cliente pode navegar a partir deste recurso.</summary>
/// <param name="Href">URL absoluta.</param>
/// <param name="Rel">Relação com o recurso atual (self, atualizar, remover, pet...).</param>
/// <param name="Metodo">Verbo HTTP a ser usado.</param>
public record Link(string Href, string Rel, string Metodo);
