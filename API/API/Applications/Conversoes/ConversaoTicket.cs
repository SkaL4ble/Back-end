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
                Ativo = ticket.Ativo,
                Prioridade = ticket.Prioridade,

                DataCriacao = ticket.DataCriacao,
                DataPrazo = ticket.DataPrazo,
                DataEntrega = ticket.DataEntrega,

                UsuarioResponsavel = ticket.UsuarioResponsavel,
                NomeUsuarioResponsavel = ticket.UsuarioResponsavelNavigation?.NomeUsuario,

                Colaboradores = ticket.Usuario.Select(u => new ColaboradorTicketDto
                {
                    UsuarioId = u.UsuarioId,
                    NomeUsuario = u.NomeUsuario,
                    ImagemUsuario = u.Imagem is null ? null : Convert.ToBase64String(u.Imagem)
                }).ToList(),

                ProjetoId = ticket.ProjetoId,
                NomeProjeto = ticket.Projeto?.NomeProjeto ?? string.Empty,

                TipoTicketId = ticket.TipoTicketId,
                NomeTipoTicket = ticket.TipoTicket?.NomeTipoTicket ?? string.Empty,
                StatusTicketId = ticket.StatusTicketId,
                NomeStatusTicket = ticket.StatusTicket?.NomeStatusTicket ?? string.Empty
            };
        }

        public static Ticket ConverterParaDomain(CriarTicketDto ticketDto)
        {
            return new Ticket
            {
                NomeTicket = ticketDto.NomeTicket,
                Prioridade = ticketDto.Prioridade,
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
            ticket.UsuarioResponsavel = ticketDto.UsuarioResponsavel;
            ticket.ProjetoId = ticketDto.ProjetoId;
            ticket.TipoTicketId = ticketDto.TipoTicketId;
            ticket.StatusTicketId = ticketDto.StatusTicketId;
        }
    }
}