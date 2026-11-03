using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Dominio.Comum;
using PetHealthAPI.Dominio.Filtros;

namespace PetHealthAPI.Aplicacao.Interfaces;

public interface IMedicamentoServico
{
    Task<ResultadoPaginado<MedicamentoResponse>> ListarAsync(FiltroMedicamento filtro);
    Task<MedicamentoResponse> ObterPorIdAsync(int id);
    Task<MedicamentoResponse> CriarAsync(CriarMedicamentoRequest request);
    Task AtualizarAsync(int id, AtualizarMedicamentoRequest request);
    Task<MedicamentoResponse> EncerrarAsync(int id, EncerrarMedicamentoRequest request);
    Task RemoverAsync(int id);
}
