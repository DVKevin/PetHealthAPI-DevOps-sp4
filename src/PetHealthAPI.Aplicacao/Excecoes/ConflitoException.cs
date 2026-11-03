namespace PetHealthAPI.Aplicacao.Excecoes;

/// <summary>Conflito com o estado atual (ex: email duplicado). O handler global converte em HTTP 409.</summary>
public class ConflitoException : Exception
{
    public ConflitoException(string mensagem) : base(mensagem) { }
}
