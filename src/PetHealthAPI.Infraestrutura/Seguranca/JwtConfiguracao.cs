namespace PetHealthAPI.Infraestrutura.Seguranca;

/// <summary>Seção "Jwt" do appsettings.json (a chave pode ser sobrescrita via user-secrets).</summary>
public class JwtConfiguracao
{
    public const string Secao = "Jwt";

    public string Emissor { get; set; } = "PetHealthAPI";
    public string Audiencia { get; set; } = "PetHealthClientes";
    public string Chave { get; set; } = string.Empty;
    public int ExpiracaoMinutos { get; set; } = 60;
}
