namespace PetHealthAPI.Hateoas;

/// <summary>Página de recursos com metadados de paginação e links de navegação (primeira, anterior, próxima, última).</summary>
public record PaginaComLinks<T>(
    IReadOnlyList<RecursoComLinks<T>> Itens,
    int Pagina,
    int TamanhoPagina,
    int TotalItens,
    int TotalPaginas,
    IReadOnlyList<Link> Links);
