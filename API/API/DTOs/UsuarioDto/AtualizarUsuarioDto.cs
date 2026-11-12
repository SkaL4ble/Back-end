namespace API.DTOs.UsuarioDto
{
    public class AtualizarUsuarioDto
    {
        public string NomeUsuario { get; set; } = null!;

        public string EmailPessoal { get; set; } = null!;

        public int CargoId { get; set; }

        public int AtuacaoId { get; set; }

        public byte[]? Imagem { get; set; }
    }
}