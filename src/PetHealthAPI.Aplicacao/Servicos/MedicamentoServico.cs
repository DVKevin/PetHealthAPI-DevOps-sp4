using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Aplicacao.Excecoes;
using PetHealthAPI.Aplicacao.Interfaces;
using PetHealthAPI.Aplicacao.Mapeamentos;
using PetHealthAPI.Dominio.Comum;
using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Filtros;
using PetHealthAPI.Dominio.Interfaces;

namespace PetHealthAPI.Aplicacao.Servicos;

public class MedicamentoServico : IMedicamentoServico
{
    private readonly IMedicamentoRepositorio _medicamentoRepositorio;
    private readonly IPetRepositorio _petRepositorio;

    public MedicamentoServico(IMedicamentoRepositorio medicamentoRepositorio, IPetRepositorio petRepositorio)
    {
        _medicamentoRepositorio = medicamentoRepositorio;
        _petRepositorio = petRepositorio;
    }

    public async Task<ResultadoPaginado<MedicamentoResponse>> ListarAsync(FiltroMedicamento filtro)
    {
        var resultado = await _medicamentoRepositorio.ListarAsync(filtro);
        return resultado.Mapear(medicamento => medicamento.ParaResponse());
    }

    public async Task<MedicamentoResponse> ObterPorIdAsync(int id)
    {
        var medicamento = await ObterEntidadeAsync(id);
        return medicamento.ParaResponse();
    }

    public async Task<MedicamentoResponse> CriarAsync(CriarMedicamentoRequest request)
    {
        var medicamento = new Medicamento(request.Nome, request.Dosagem, request.Frequencia, request.DataInicio,
                                          request.DataFim, request.VeterinarioPrescreveu, request.Observacoes,
                                          request.PetId);

        await ValidacaoPet.GarantirPetExisteAsync(_petRepositorio, medicamento.PetId);

        await _medicamentoRepositorio.AdicionarAsync(medicamento);
        return medicamento.ParaResponse();
    }

    public async Task AtualizarAsync(int id, AtualizarMedicamentoRequest request)
    {
        var medicamento = await ObterEntidadeAsync(id);
        medicamento.Atualizar(request.Nome, request.Dosagem, request.Frequencia, request.DataInicio,
                              request.DataFim, request.VeterinarioPrescreveu, request.Observacoes, request.Ativo);

        await _medicamentoRepositorio.AtualizarAsync(medicamento);
    }

    public async Task<MedicamentoResponse> EncerrarAsync(int id, EncerrarMedicamentoRequest request)
    {
        var medicamento = await ObterEntidadeAsync(id);
        medicamento.Encerrar(request.DataFim ?? DateTime.Now);

        await _medicamentoRepositorio.AtualizarAsync(medicamento);
        return medicamento.ParaResponse();
    }

    public async Task RemoverAsync(int id)
    {
        var medicamento = await ObterEntidadeAsync(id);
        await _medicamentoRepositorio.RemoverAsync(medicamento);
    }

    private async Task<Medicamento> ObterEntidadeAsync(int id)
    {
        return await _medicamentoRepositorio.ObterPorIdAsync(id)
               ?? throw new RecursoNaoEncontradoException($"Medicamento com ID {id} não encontrado.");
    }
}
