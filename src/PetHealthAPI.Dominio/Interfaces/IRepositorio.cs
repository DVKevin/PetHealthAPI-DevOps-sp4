using PetHealthAPI.Dominio.Comum;

namespace PetHealthAPI.Dominio.Interfaces;

/// <summary>
/// Contrato base de persistência. Cada agregado estende com suas consultas específicas (ISP).
/// </summary>
public interface IRepositorio<TEntidade, TId, TFiltro>
    where TEntidade : class
    where TFiltro : ParametrosConsulta
{
    Task<TEntidade?> ObterPorIdAsync(TId id);
    Task<ResultadoPaginado<TEntidade>> ListarAsync(TFiltro filtro);
    Task AdicionarAsync(TEntidade entidade);
    Task AtualizarAsync(TEntidade entidade);
    Task RemoverAsync(TEntidade entidade);
}
