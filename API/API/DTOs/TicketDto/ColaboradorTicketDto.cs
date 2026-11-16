namespace API.DTOs.TicketDto
{
    public class ColaboradorTicketDto
    {
        public Guid UsuarioId { get; set; }

        public string NomeUsuario { get; set; } = null!;

        public string? ImagemUsuario { get; set; }
    }
}