using System.Security.Cryptography;
using System.Text;

namespace API.Applications.Autenticacao
{
    public class CriptografiaUsuario
    {
        public static Byte[] CriptografarSenha(string senha)
        {
            SHA256 sha256 = SHA256.Create();

            byte[] senhaBytes = Encoding.UTF8.GetBytes(senha);
            byte[] SenhaCriptografada = sha256.ComputeHash(senhaBytes);

            return SenhaCriptografada;
        }
    }
}
