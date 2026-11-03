namespace PetHealthAPI.Dominio.Comum;

/// <summary>
/// Parâmetros comuns de paginação e ordenação recebidos via query string.
/// </summary>
public abstract class ParametrosConsulta
{
    public const int TamanhoPaginaPadrao = 10;
    public const int TamanhoPaginaMaximo = 50;

    private int _pagina = 1;
    private int _tamanhoPagina = TamanhoPaginaPadrao;

    /// <summary>Número da página (começa em 1).</summary>
    public int Pagina
    {
        get => _pagina;
        set => _pagina = value < 1 ? 1 : value;
    }

    /// <summary>Itens por página (máximo 50).</summary>
    public int TamanhoPagina
    {
        get => _tamanhoPagina;
        set => _tamanhoPagina = value < 1 ? TamanhoPaginaPadrao : Math.Min(value, TamanhoPaginaMaximo);
    }

    /// <summary>Campo usado na ordenação (ex: nome, dataCadastro).</summary>
    public string? OrdenarPor { get; set; }

    /// <summary>Direção da ordenação: asc ou desc.</summary>
    public string? Direcao { get; set; } = "asc";

    public bool EhDescendente() =>
        string.Equals(Direcao, "desc", StringComparison.OrdinalIgnoreCase);
}
