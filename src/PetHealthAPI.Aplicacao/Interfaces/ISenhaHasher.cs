namespace PetHealthAPI.Aplicacao.Interfaces;

/// <summary>Abstração do algoritmo de hash de senha (implementado na Infraestrutura).</summary>
public interface ISenhaHasher
{
    string GerarHash(string senha);
    bool Verificar(string senhaHash, string senha);
}
