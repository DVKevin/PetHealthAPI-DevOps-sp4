using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Dominio.Comum;
using PetHealthAPI.Dominio.Filtros;

namespace PetHealthAPI.Aplicacao.Interfaces;

public interface IVacinaServico
{
    Task<ResultadoPaginado<VacinaResponse>> ListarAsync(FiltroVacina filtro);
    Task<VacinaResponse> ObterPorIdAsync(int id);
    Task<VacinaResponse> CriarAsync(CriarVacinaRequest request);
    Task AtualizarAsync(int id, AtualizarVacinaRequest request);
    Task RemoverAsync(int id);
}
