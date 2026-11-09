using API.Domains;
using API.DTOs.TipoTicketDto;

namespace API.Applications.Conversoes.ConversoesDeDto
{
    public class ConversaoTipoTicket
    {
        public static ListarTipoTicketDto ConverterParaDto(TipoTicket tipoTicket)
        {
            return new ListarTipoTicketDto
            {
                TipoTicketId = tipoTicket.TipoTicketId,
                NomeTipoTicket = tipoTicket.NomeTipoTicket
            };
        }

        public static TipoTicket ConverterParaDomain(CriarTipoTicketDto tipoTicketDto)
        {
            return new TipoTicket
            {
                NomeTipoTicket = tipoTicketDto.NomeTipoTicket
            };
        }
    }
}