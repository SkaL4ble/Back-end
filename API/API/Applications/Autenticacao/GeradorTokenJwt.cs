using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Domains;
using API.Exceptions;
using Microsoft.IdentityModel.Tokens;

namespace API.Applications.Autenticacao
{
    public class GeradorTokenJwt
    {
        public string GerarToken(Usuario usuario)
        {
            string? chave = Environment.GetEnvironmentVariable("JWT_KEY");
            string? issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
            string? audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
            string? expiracao = Environment.GetEnvironmentVariable(
                "JWT_EXPIRA_EM_MINUTOS");

            if (string.IsNullOrWhiteSpace(chave) ||
                string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience))
            {
                throw new DomainException(
                    "As configurações JWT estão incompletas.");
            }

            if (!int.TryParse(expiracao, out int minutos) || minutos <= 0)
            {
                throw new DomainException(
                    "JWT_EXPIRA_EM_MINUTOS deve ser um número positivo.");
            }

            byte[] keyBytes = Encoding.UTF8.GetBytes(chave);

            if (keyBytes.Length < 32)
            {
                throw new DomainException(
                    "JWT_KEY precisa ter pelo menos 32 bytes.");
            }

            var securityKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    usuario.UsuarioId.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    usuario.NomeUsuario),

                new Claim(
                    ClaimTypes.Email,
                    usuario.EmailPessoal),

                new Claim(
                     ClaimTypes.Role,
                    usuario.TipoUsuario.NomeTipoUsuario)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(minutos),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}