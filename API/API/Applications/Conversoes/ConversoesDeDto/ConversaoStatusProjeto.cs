using API.Domains;
using API.DTOs.StatusProjetoDto;

namespace API.Applications.Conversoes.ConversoesDeDto
{
    public class ConversaoStatusProjeto
    {
        public static ListarStatusProjetoDto ConverterParaDto(StatusProjeto statusProjeto)
        {
            return new ListarStatusProjetoDto
            {
                StatusProjetoId = statusProjeto.StatusProjetoId,
                NomeStatusProjeto = statusProjeto.NomeStatusProjeto
            };
        }

        public static StatusProjeto ConverterParaDomain(CriarStatusProjetoDto statusProjetoDto)
        {
            return new StatusProjeto
            {
                NomeStatusProjeto = statusProjetoDto.NomeStatusProjeto
            };
        }
    }
}