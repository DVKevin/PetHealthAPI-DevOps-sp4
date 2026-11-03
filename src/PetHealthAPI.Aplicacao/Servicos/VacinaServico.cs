using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Aplicacao.Excecoes;
using PetHealthAPI.Aplicacao.Interfaces;
using PetHealthAPI.Aplicacao.Mapeamentos;
using PetHealthAPI.Dominio.Comum;
using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Filtros;
using PetHealthAPI.Dominio.Interfaces;

namespace PetHealthAPI.Aplicacao.Servicos;

public class VacinaServico : IVacinaServico
{
    private readonly IVacinaRepositorio _vacinaRepositorio;
    private readonly IPetRepositorio _petRepositorio;

    public VacinaServico(IVacinaRepositorio vacinaRepositorio, IPetRepositorio petRepositorio)
    {
        _vacinaRepositorio = vacinaRepositorio;
        _petRepositorio = petRepositorio;
    }

    public async Task<ResultadoPaginado<VacinaResponse>> ListarAsync(FiltroVacina filtro)
    {
        var resultado = await _vacinaRepositorio.ListarAsync(filtro);
        return resultado.Mapear(vacina => vacina.ParaResponse());
    }

    public async Task<VacinaResponse> ObterPorIdAsync(int id)
    {
        var vacina = await ObterEntidadeAsync(id);
        return vacina.ParaResponse();
    }

    public async Task<VacinaResponse> CriarAsync(CriarVacinaRequest request)
    {
        var vacina = new Vacina(request.Nome, request.DataAplicacao, request.DataProximaDose, request.Fabricante,
                                request.Lote, request.Veterinario, request.Observacoes, request.PetId);

        await ValidacaoPet.GarantirPetExisteAsync(_petRepositorio, vacina.PetId);

        await _vacinaRepositorio.AdicionarAsync(vacina);
        return vacina.ParaResponse();
    }

    public async Task AtualizarAsync(int id, AtualizarVacinaRequest request)
    {
        var vacina = await ObterEntidadeAsync(id);
        vacina.Atualizar(request.Nome, request.DataAplicacao, request.DataProximaDose, request.Fabricante,
                         request.Lote, request.Veterinario, request.Observacoes);

        await _vacinaRepositorio.AtualizarAsync(vacina);
    }

    public async Task RemoverAsync(int id)
    {
        var vacina = await ObterEntidadeAsync(id);
        await _vacinaRepositorio.RemoverAsync(vacina);
    }

    private async Task<Vacina> ObterEntidadeAsync(int id)
    {
        return await _vacinaRepositorio.ObterPorIdAsync(id)
               ?? throw new RecursoNaoEncontradoException($"Vacina com ID {id} não encontrada.");
    }
}
