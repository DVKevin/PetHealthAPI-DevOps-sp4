using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PetHealthAPI.Aplicacao.Interfaces;
using PetHealthAPI.Dominio.Entidades;

namespace PetHealthAPI.Infraestrutura.Seguranca;

/// <summary>Emite tokens JWT assinados com HMAC-SHA256, contendo id, nome, email e perfil (role).</summary>
public class GeradorTokenJwt : IGeradorToken
{
    private readonly JwtConfiguracao _configuracao;

    public GeradorTokenJwt(IOptions<JwtConfiguracao> configuracao)
    {
        _configuracao = configuracao.Value;
    }

    public TokenGerado Gerar(Usuario usuario)
    {
        var agora = DateTime.UtcNow;
        var expiraEm = agora.AddMinutes(_configuracao.ExpiracaoMinutos);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Role, usuario.Perfil)
        };

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuracao.Chave));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuracao.Emissor,
            audience: _configuracao.Audiencia,
            claims: claims,
            notBefore: agora,
            expires: expiraEm,
            signingCredentials: credenciais);

        return new TokenGerado(new JwtSecurityTokenHandler().WriteToken(token), expiraEm);
    }
}
