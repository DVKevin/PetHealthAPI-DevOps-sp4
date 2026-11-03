namespace PetHealthAPI.Tests.Integration.Infraestrutura;

/// <summary>Contratos mínimos para ler as respostas HATEOAS da API nos testes.</summary>
public record LinkResposta(string Href, string Rel, string Metodo);

public record RecursoResposta<T>(T Dados, List<LinkResposta> Links);

public record PaginaResposta<T>(List<RecursoResposta<T>> Itens, int Pagina, int TamanhoPagina,
                                int TotalItens, int TotalPaginas, List<LinkResposta> Links);

public record TutorResposta(int Id, string Nome, string Email);

public record PetResposta(int Id, string Nome, int TutorId);

public record ProblemaResposta(int Status, string Title, string Detail);

public record TokenResposta(string Token, string Tipo, DateTime ExpiraEm, string Nome, string Email, string Perfil);
