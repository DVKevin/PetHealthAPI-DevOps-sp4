using PetHealthAPI.Dominio.Interfaces;

namespace PetHealthAPI.Aplicacao.Servicos;

/// <summary>
/// Regra de aplicação reutilizada por vacinas, consultas, medicamentos e diário:
/// todo registro clínico precisa apontar para um pet existente.
/// </summary>
internal static class ValidacaoPet
{
    public static async Task GarantirPetExisteAsync(IPetRepositorio petRepositorio, int petId)
    {
        if (!await petRepositorio.ExisteAsync(petId))
            throw new ArgumentException($"Pet com ID {petId} não encontrado.", nameof(petId));
    }
}
