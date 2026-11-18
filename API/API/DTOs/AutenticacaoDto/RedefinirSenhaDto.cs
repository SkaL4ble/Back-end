namespace API.DTOs.AutenticacaoDto
{
    public class RedefinirSenhaDto
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string NovaSenha { get; set; } = string.Empty;
    }
}
