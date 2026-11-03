namespace PetHealthAPI.Aplicacao.Excecoes;

/// <summary>Login com email ou senha incorretos. O handler global converte em HTTP 401.</summary>
public class CredenciaisInvalidasException : Exception
{
    public const string MensagemPadrao = "Email ou senha inválidos.";

    public CredenciaisInvalidasException() : base(MensagemPadrao) { }
}
