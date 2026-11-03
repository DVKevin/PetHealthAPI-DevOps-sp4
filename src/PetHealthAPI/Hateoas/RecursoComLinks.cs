namespace PetHealthAPI.Hateoas;

/// <summary>Um recurso acompanhado dos seus links HATEOAS.</summary>
public record RecursoComLinks<T>(T Dados, IReadOnlyList<Link> Links);
