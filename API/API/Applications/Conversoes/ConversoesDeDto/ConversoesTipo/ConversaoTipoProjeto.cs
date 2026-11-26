using API.Domains;
using API.DTOs.TipoDto.TipoProjetoDto;

namespace API.Applications.Conversoes.ConversoesDeDto.ConversoesTipo
{
    public class ConversaoTipoProjeto
    {
        public static ListarTipoProjetoDto ConverterParaDto(TipoProjeto tipoProjeto)
        {
            return new ListarTipoProjetoDto
            {
                TipoProjetoId = tipoProjeto.TipoProjetoId,
                NomeTipoProjeto = tipoProjeto.NomeTipoProjeto
            };
        }

        public static TipoProjeto ConverterParaDomain(CriarTipoProjetoDto tipoProjetoDto)
        {
            return new TipoProjeto
            {
                NomeTipoProjeto = tipoProjetoDto.NomeTipoProjeto
            };
        }
    }
}