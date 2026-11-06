namespace API.DTOs.TicketDto
{
    public class ListarTicketDto
    {
        public int TicketId { get; set; }

        public string NomeTicket { get; set; } = null!;

        public int Prioridade { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime? DataPrazo { get; set; }

        public DateTime? DataEntrega { get; set; }

        public bool Ativo { get; set; }

        public Guid? UsuarioResponsavel { get; set; }
        public string? NomeUsuarioResponsavel { get; set; }

        public Guid ProjetoId { get; set; }
        public string NomeProjeto { get; set; } = null!;

        public int TipoTicketId { get; set; }
        public string NomeTipoTicket { get; set; } = null!;

        public int StatusTicketId { get; set; }
        public string NomeStatusTicket { get; set; } = null!;
    }
}
