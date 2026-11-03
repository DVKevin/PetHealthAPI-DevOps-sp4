using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Dominio.Comum;
using PetHealthAPI.Dominio.Filtros;

namespace PetHealthAPI.Aplicacao.Interfaces;

public interface IConsultaServico
{
    Task<ResultadoPaginado<ConsultaResponse>> ListarAsync(FiltroConsulta filtro);
    Task<ConsultaResponse> ObterPorIdAsync(int id);
    Task<ConsultaResponse> CriarAsync(CriarConsultaRequest request);
    Task AtualizarAsync(int id, AtualizarConsultaRequest request);
    Task RemoverAsync(int id);
}
