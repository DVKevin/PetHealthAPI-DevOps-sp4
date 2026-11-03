using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Aplicacao.Excecoes;
using PetHealthAPI.Aplicacao.Interfaces;
using PetHealthAPI.Aplicacao.Mapeamentos;
using PetHealthAPI.Dominio.Comum;
using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Filtros;
using PetHealthAPI.Dominio.Interfaces;

namespace PetHealthAPI.Aplicacao.Servicos;

public class ConsultaServico : IConsultaServico
{
    private readonly IConsultaRepositorio _consultaRepositorio;
    private readonly IPetRepositorio _petRepositorio;

    public ConsultaServico(IConsultaRepositorio consultaRepositorio, IPetRepositorio petRepositorio)
    {
        _consultaRepositorio = consultaRepositorio;
        _petRepositorio = petRepositorio;
    }

    public async Task<ResultadoPaginado<ConsultaResponse>> ListarAsync(FiltroConsulta filtro)
    {
        var resultado = await _consultaRepositorio.ListarAsync(filtro);
        return resultado.Mapear(consulta => consulta.ParaResponse());
    }

    public async Task<ConsultaResponse> ObterPorIdAsync(int id)
    {
        var consulta = await ObterEntidadeAsync(id);
        return consulta.ParaResponse();
    }

    public async Task<ConsultaResponse> CriarAsync(CriarConsultaRequest request)
    {
        var consulta = new Consulta(request.DataConsulta, request.Motivo, request.Veterinario, request.Clinica,
                                    request.Diagnostico, request.Tratamento, request.Custo, request.DataRetorno,
                                    request.Observacoes, request.PetId);

        await ValidacaoPet.GarantirPetExisteAsync(_petRepositorio, consulta.PetId);

        await _consultaRepositorio.AdicionarAsync(consulta);
        return consulta.ParaResponse();
    }

    public async Task AtualizarAsync(int id, AtualizarConsultaRequest request)
    {
        var consulta = await ObterEntidadeAsync(id);
        consulta.Atualizar(request.DataConsulta, request.Motivo, request.Veterinario, request.Clinica,
                           request.Diagnostico, request.Tratamento, request.Custo, request.DataRetorno,
                           request.Observacoes);

        await _consultaRepositorio.AtualizarAsync(consulta);
    }

    public async Task RemoverAsync(int id)
    {
        var consulta = await ObterEntidadeAsync(id);
        await _consultaRepositorio.RemoverAsync(consulta);
    }

    private async Task<Consulta> ObterEntidadeAsync(int id)
    {
        return await _consultaRepositorio.ObterPorIdAsync(id)
               ?? throw new RecursoNaoEncontradoException($"Consulta com ID {id} não encontrada.");
    }
}
