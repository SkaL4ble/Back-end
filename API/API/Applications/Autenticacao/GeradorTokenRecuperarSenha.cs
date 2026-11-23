using System.Security.Cryptography;
using System.Text;

namespace API.Applications.Autenticacao
{
    public class GeradorTokenRecuperacaoSenha
    {
        // Gera um token aleatório para enviar no e-mail
        public string GerarToken()
        {
            var token = RandomNumberGenerator.GetBytes(32);

            return Convert.ToBase64String(token);
        }

        // Gera o hash do token para salvar no banco
        public string GerarHashToken(string token)
        {
            using var sha = SHA256.Create();

            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = sha.ComputeHash(bytes);

            return Convert.ToHexString(hash);
        }
    }
}