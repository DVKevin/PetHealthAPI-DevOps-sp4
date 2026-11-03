namespace PetHealthAPI.Aplicacao.Excecoes;

/// <summary>Recurso inexistente. O handler global converte em HTTP 404.</summary>
public class RecursoNaoEncontradoException : Exception
{
    public RecursoNaoEncontradoException(string mensagem) : base(mensagem) { }
}
