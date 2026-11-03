namespace PetHealthAPI.Dominio.Constantes;

/// <summary>
/// Perfis de acesso usados na autorização (roles do JWT).
/// </summary>
public static class PerfisUsuario
{
    public const string Admin = "Admin";
    public const string Usuario = "Usuario";

    public static readonly IReadOnlyList<string> Todos = new[] { Admin, Usuario };
}
