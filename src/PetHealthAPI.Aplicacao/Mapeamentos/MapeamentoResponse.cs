using PetHealthAPI.Aplicacao.Dtos;
using PetHealthAPI.Dominio.Entidades;

namespace PetHealthAPI.Aplicacao.Mapeamentos;

/// <summary>
/// Converte entidades de domínio em DTOs de resposta.
/// A API nunca expõe entidades diretamente (evita referência circular no JSON e vazamento de dados).
/// </summary>
public static class MapeamentoResponse
{
    public const int DiasAlertaVacina = 30;

    public static TutorResponse ParaResponse(this Tutor tutor) =>
        new(tutor.Id, tutor.Nome, tutor.Email, tutor.Telefone, tutor.Endereco, tutor.DataCadastro);

    public static PetResponse ParaResponse(this Pet pet) =>
        new(pet.Id, pet.Nome, pet.Especie, pet.Raca, pet.Idade, pet.Peso, pet.Sexo, pet.Castrado,
            pet.Alergias, pet.DataNascimento, pet.DataCadastro, pet.TutorId, pet.Tutor?.Nome);

    public static VacinaResponse ParaResponse(this Vacina vacina) =>
        new(vacina.Id, vacina.Nome, vacina.DataAplicacao, vacina.DataProximaDose, vacina.Fabricante,
            vacina.Lote, vacina.Veterinario, vacina.Observacoes, vacina.PetId,
            vacina.ProximaDoseVenceEm(DiasAlertaVacina, DateTime.Now));

    public static ConsultaResponse ParaResponse(this Consulta consulta) =>
        new(consulta.Id, consulta.DataConsulta, consulta.Motivo, consulta.Veterinario, consulta.Clinica,
            consulta.Diagnostico, consulta.Tratamento, consulta.Custo, consulta.DataRetorno,
            consulta.Observacoes, consulta.PetId, consulta.TemRetornoPendente(DateTime.Now));

    public static MedicamentoResponse ParaResponse(this Medicamento medicamento) =>
        new(medicamento.Id, medicamento.Nome, medicamento.Dosagem, medicamento.Frequencia,
            medicamento.DataInicio, medicamento.DataFim, medicamento.VeterinarioPrescreveu,
            medicamento.Ativo, medicamento.Observacoes, medicamento.PetId);

    public static RegistroDiarioResponse ParaResponse(this RegistroDiario registro) =>
        new(registro.Id, registro.PetId, registro.Data, registro.Alimentacao, registro.Hidratacao,
            registro.Passeio, registro.MedicacaoEmDia, registro.Humor, registro.Observacoes,
            registro.Pontos, registro.DataRegistro);

    public static UsuarioResponse ParaResponse(this Usuario usuario) =>
        new(usuario.Id, usuario.Nome, usuario.Email, usuario.Perfil);
}
