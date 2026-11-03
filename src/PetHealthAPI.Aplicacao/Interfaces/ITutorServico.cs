using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Dominio.Comum;
using PetHealthAPI.Dominio.Filtros;

namespace PetHealthAPI.Aplicacao.Interfaces;

public interface ITutorServico
{
    Task<ResultadoPaginado<TutorResponse>> ListarAsync(FiltroTutor filtro);
    Task<TutorResponse> ObterPorIdAsync(int id);
    Task<TutorResponse> CriarAsync(TutorRequest request);
    Task AtualizarAsync(int id, TutorRequest request);
    Task RemoverAsync(int id);
}
