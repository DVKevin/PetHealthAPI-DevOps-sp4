namespace PetHealthAPI.Infraestrutura.Seguranca;

/// <summary>Seção "AdminPadrao": usuário Admin criado automaticamente na inicialização, se não existir.</summary>
public class AdminPadraoConfiguracao
{
    public const string Secao = "AdminPadrao";

    public string Nome { get; set; } = "Administrador";
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}
