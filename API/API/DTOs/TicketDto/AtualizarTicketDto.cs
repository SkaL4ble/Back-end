namespace API.DTOs.TicketDto
{
    public class AtualizarTicketDto
    {
        public string NomeTicket { get; set; } = null!;
        public int Prioridade { get; set; }
        public DateTime? DataPrazo { get; set; }
        public DateTime? DataEntrega { get; set; }
        public Guid? UsuarioResponsavel { get; set; }
        public Guid ProjetoId { get; set; }
        public int TipoTicketId { get; set; }
        public int StatusTicketId { get; set; }
    }
}