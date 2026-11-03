using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Aplicacao.Excecoes;
using PetHealthAPI.Aplicacao.Interfaces;
using PetHealthAPI.Aplicacao.Mapeamentos;
using PetHealthAPI.Dominio.Comum;
using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Filtros;
using PetHealthAPI.Dominio.Interfaces;

namespace PetHealthAPI.Aplicacao.Servicos;

public class TutorServico : ITutorServico
{
    public const string MensagemEmailDuplicado = "Já existe um tutor cadastrado com este email.";

    private readonly ITutorRepositorio _tutorRepositorio;

    public TutorServico(ITutorRepositorio tutorRepositorio)
    {
        _tutorRepositorio = tutorRepositorio;
    }

    public async Task<ResultadoPaginado<TutorResponse>> ListarAsync(FiltroTutor filtro)
    {
        var resultado = await _tutorRepositorio.ListarAsync(filtro);
        return resultado.Mapear(tutor => tutor.ParaResponse());
    }

    public async Task<TutorResponse> ObterPorIdAsync(int id)
    {
        var tutor = await ObterEntidadeAsync(id);
        return tutor.ParaResponse();
    }

    public async Task<TutorResponse> CriarAsync(TutorRequest request)
    {
        var tutor = new Tutor(request.Nome, request.Email, request.Telefone, request.Endereco);

        if (await _tutorRepositorio.EmailEmUsoAsync(tutor.Email, null))
            throw new ConflitoException(MensagemEmailDuplicado);

        await _tutorRepositorio.AdicionarAsync(tutor);
        return tutor.ParaResponse();
    }

    public async Task AtualizarAsync(int id, TutorRequest request)
    {
        var tutor = await ObterEntidadeAsync(id);
        tutor.Atualizar(request.Nome, request.Email, request.Telefone, request.Endereco);

        if (await _tutorRepositorio.EmailEmUsoAsync(tutor.Email, id))
            throw new ConflitoException(MensagemEmailDuplicado);

        await _tutorRepositorio.AtualizarAsync(tutor);
    }

    public async Task RemoverAsync(int id)
    {
        var tutor = await ObterEntidadeAsync(id);
        await _tutorRepositorio.RemoverAsync(tutor);
    }

    private async Task<Tutor> ObterEntidadeAsync(int id)
    {
        return await _tutorRepositorio.ObterPorIdAsync(id)
               ?? throw new RecursoNaoEncontradoException($"Tutor com ID {id} não encontrado.");
    }
}
