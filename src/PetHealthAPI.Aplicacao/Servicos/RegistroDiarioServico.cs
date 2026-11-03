using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Aplicacao.Excecoes;
using PetHealthAPI.Aplicacao.Interfaces;
using PetHealthAPI.Aplicacao.Mapeamentos;
using PetHealthAPI.Dominio.Comum;
using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Filtros;
using PetHealthAPI.Dominio.Interfaces;

namespace PetHealthAPI.Aplicacao.Servicos;

/// <summary>
/// Diário de cuidados: os registros ficam no MongoDB, mas o pet é validado no Oracle
/// (integração entre persistência relacional e NoSQL).
/// </summary>
public class RegistroDiarioServico : IRegistroDiarioServico
{
    public const int PontosPorNivel = 100;

    private readonly IRegistroDiarioRepositorio _registroRepositorio;
    private readonly IPetRepositorio _petRepositorio;

    public RegistroDiarioServico(IRegistroDiarioRepositorio registroRepositorio, IPetRepositorio petRepositorio)
    {
        _registroRepositorio = registroRepositorio;
        _petRepositorio = petRepositorio;
    }

    public async Task<ResultadoPaginado<RegistroDiarioResponse>> ListarAsync(FiltroRegistroDiario filtro)
    {
        var resultado = await _registroRepositorio.ListarAsync(filtro);
        return resultado.Mapear(registro => registro.ParaResponse());
    }

    public async Task<RegistroDiarioResponse> ObterPorIdAsync(string id)
    {
        var registro = await ObterEntidadeAsync(id);
        return registro.ParaResponse();
    }

    public async Task<RegistroDiarioResponse> CriarAsync(CriarRegistroDiarioRequest request)
    {
        var registro = new RegistroDiario(request.PetId, request.Data, request.Alimentacao, request.Hidratacao,
                                          request.Passeio, request.MedicacaoEmDia, request.Humor, request.Observacoes);

        await ValidacaoPet.GarantirPetExisteAsync(_petRepositorio, registro.PetId);

        await _registroRepositorio.AdicionarAsync(registro);
        return registro.ParaResponse();
    }

    public async Task AtualizarAsync(string id, AtualizarRegistroDiarioRequest request)
    {
        var registro = await ObterEntidadeAsync(id);
        registro.Atualizar(request.Data, request.Alimentacao, request.Hidratacao, request.Passeio,
                           request.MedicacaoEmDia, request.Humor, request.Observacoes);

        await _registroRepositorio.AtualizarAsync(registro);
    }

    public async Task RemoverAsync(string id)
    {
        var registro = await ObterEntidadeAsync(id);
        await _registroRepositorio.RemoverAsync(registro);
    }

    public async Task<ResumoPontosResponse> ObterResumoPontosAsync(int petId)
    {
        if (!await _petRepositorio.ExisteAsync(petId))
            throw new RecursoNaoEncontradoException($"Pet com ID {petId} não encontrado.");

        var totalPontos = await _registroRepositorio.SomarPontosDoPetAsync(petId);
        var nivel = (totalPontos / PontosPorNivel) + 1;
        var pontosParaProximoNivel = (nivel * PontosPorNivel) - totalPontos;

        return new ResumoPontosResponse(petId, totalPontos, nivel, pontosParaProximoNivel);
    }

    private async Task<RegistroDiario> ObterEntidadeAsync(string id)
    {
        return await _registroRepositorio.ObterPorIdAsync(id)
               ?? throw new RecursoNaoEncontradoException($"Registro diário com ID {id} não encontrado.");
    }
}
