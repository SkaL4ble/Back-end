using API.Domains;
using API.DTOs.TicketDto;

namespace API.Applications.Conversoes
{
    public class ConversaoTicket
    {
        public static ListarTicketDto ConverterParaDto(Ticket ticket)
        {
            return new ListarTicketDto
            {
                TicketId = ticket.TicketId,
                NomeTicket = ticket.NomeTicket,
                Prioridade = ticket.Prioridade,
                DataCriacao = ticket.DataCriacao,
                DataPrazo = ticket.DataPrazo,
                DataEntrega = ticket.DataEntrega,
                Ativo = ticket.Ativo,
                UsuarioResponsavel = ticket.UsuarioResponsavel,
                ProjetoId = ticket.ProjetoId,
                TipoTicketId = ticket.TipoTicketId,
                StatusTicketId = ticket.StatusTicketId
            };
        }

        public static Ticket ConverterParaDomain(CriarTicketDto ticketDto)
        {
            return new Ticket
            {
                NomeTicket = ticketDto.NomeTicket,
                Prioridade = ticketDto.Prioridade,
                DataCriacao = DateTime.UtcNow,
                DataPrazo = ticketDto.DataPrazo,
                Ativo = true,
                UsuarioResponsavel = ticketDto.UsuarioResponsavel,
                ProjetoId = ticketDto.ProjetoId,
                TipoTicketId = ticketDto.TipoTicketId,
                StatusTicketId = ticketDto.StatusTicketId
            };
        }

        public static void AplicarAtualizacao(Ticket ticket, AtualizarTicketDto ticketDto)
        {
            ticket.NomeTicket = ticketDto.NomeTicket;
            ticket.Prioridade = ticketDto.Prioridade;
            ticket.DataPrazo = ticketDto.DataPrazo;
            ticket.DataEntrega = ticketDto.DataEntrega;
            ticket.Ativo = ticketDto.Ativo;
            ticket.UsuarioResponsavel = ticketDto.UsuarioResponsavel;
            ticket.ProjetoId = ticketDto.ProjetoId;
            ticket.TipoTicketId = ticketDto.TipoTicketId;
            ticket.StatusTicketId = ticketDto.StatusTicketId;
        }
    }
}