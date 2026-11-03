using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Aplicacao.Excecoes;
using PetHealthAPI.Aplicacao.Interfaces;
using PetHealthAPI.Aplicacao.Mapeamentos;
using PetHealthAPI.Dominio.Comum;
using PetHealthAPI.Dominio.Entidades;
using PetHealthAPI.Dominio.Filtros;
using PetHealthAPI.Dominio.Interfaces;

namespace PetHealthAPI.Aplicacao.Servicos;

public class PetServico : IPetServico
{
    private readonly IPetRepositorio _petRepositorio;
    private readonly ITutorRepositorio _tutorRepositorio;

    public PetServico(IPetRepositorio petRepositorio, ITutorRepositorio tutorRepositorio)
    {
        _petRepositorio = petRepositorio;
        _tutorRepositorio = tutorRepositorio;
    }

    public async Task<ResultadoPaginado<PetResponse>> ListarAsync(FiltroPet filtro)
    {
        var resultado = await _petRepositorio.ListarAsync(filtro);
        return resultado.Mapear(pet => pet.ParaResponse());
    }

    public async Task<PetResponse> ObterPorIdAsync(int id)
    {
        var pet = await ObterEntidadeAsync(id);
        return pet.ParaResponse();
    }

    public async Task<PetResponse> CriarAsync(PetRequest request)
    {
        // 1º regra de domínio (a entidade se valida); 2º regra de aplicação (tutor existe)
        var pet = new Pet(request.Nome, request.Especie, request.Raca, request.Idade, request.Peso,
                          request.Sexo, request.Castrado, request.Alergias, request.DataNascimento, request.TutorId);

        await GarantirTutorExisteAsync(pet.TutorId);

        await _petRepositorio.AdicionarAsync(pet);
        return pet.ParaResponse();
    }

    public async Task AtualizarAsync(int id, PetRequest request)
    {
        var pet = await ObterEntidadeAsync(id);

        if (pet.TutorId != request.TutorId)
            await GarantirTutorExisteAsync(request.TutorId);

        pet.Atualizar(request.Nome, request.Especie, request.Raca, request.Idade, request.Peso,
                      request.Sexo, request.Castrado, request.Alergias, request.DataNascimento, request.TutorId);

        await _petRepositorio.AtualizarAsync(pet);
    }

    public async Task RemoverAsync(int id)
    {
        var pet = await ObterEntidadeAsync(id);
        await _petRepositorio.RemoverAsync(pet);
    }

    private async Task GarantirTutorExisteAsync(int tutorId)
    {
        if (!await _tutorRepositorio.ExisteAsync(tutorId))
            throw new ArgumentException(
                $"Tutor com ID {tutorId} não encontrado. Cadastre o tutor antes do pet.", nameof(tutorId));
    }

    private async Task<Pet> ObterEntidadeAsync(int id)
    {
        return await _petRepositorio.ObterPorIdAsync(id)
               ?? throw new RecursoNaoEncontradoException($"Pet com ID {id} não encontrado.");
    }
}
