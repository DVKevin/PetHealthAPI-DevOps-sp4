using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Dominio.Comum;
using PetHealthAPI.Dominio.Filtros;

namespace PetHealthAPI.Aplicacao.Interfaces;

public interface IPetServico
{
    Task<ResultadoPaginado<PetResponse>> ListarAsync(FiltroPet filtro);
    Task<PetResponse> ObterPorIdAsync(int id);
    Task<PetResponse> CriarAsync(PetRequest request);
    Task AtualizarAsync(int id, PetRequest request);
    Task RemoverAsync(int id);
}
