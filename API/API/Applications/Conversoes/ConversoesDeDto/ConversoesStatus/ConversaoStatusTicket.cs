using API.Domains;
using API.DTOs.StatusDto.StatusTicketDto;

namespace API.Applications.Conversoes.ConversoesDeDto.ConversoesStatus
{
    public class ConversaoStatusTicket
    {
        public static ListarStatusTicketDto ConverterParaDto(StatusTicket statusTicket)
        {
            return new ListarStatusTicketDto
            {
                StatusTicketId = statusTicket.StatusTicketId,
                NomeStatusTicket = statusTicket.NomeStatusTicket
            };
        }

        public static StatusTicket ConverterParaDomain(CriarStatusTicketDto statusTicketDto)
        {
            return new StatusTicket
            {
                NomeStatusTicket = statusTicketDto.NomeStatusTicket
            };
        }
    }
}