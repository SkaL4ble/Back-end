namespace API.DTOs.GameficacaoDto
{
    public class DemandaTipoTicketDto
    {
        public int TipoTicketId { get; set; }

        public string NomeTipoTicket { get; set; } = string.Empty;

        public int QuantidadeTickets { get; set; }

        public int QuantidadeProjetos { get; set; }
    }
}
