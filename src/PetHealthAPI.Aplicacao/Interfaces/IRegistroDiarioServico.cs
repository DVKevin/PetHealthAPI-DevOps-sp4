using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Dominio.Comum;
using PetHealthAPI.Dominio.Filtros;

namespace PetHealthAPI.Aplicacao.Interfaces;

public interface IRegistroDiarioServico
{
    Task<ResultadoPaginado<RegistroDiarioResponse>> ListarAsync(FiltroRegistroDiario filtro);
    Task<RegistroDiarioResponse> ObterPorIdAsync(string id);
    Task<RegistroDiarioResponse> CriarAsync(CriarRegistroDiarioRequest request);
    Task AtualizarAsync(string id, AtualizarRegistroDiarioRequest request);
    Task RemoverAsync(string id);
    Task<ResumoPontosResponse> ObterResumoPontosAsync(int petId);
}
