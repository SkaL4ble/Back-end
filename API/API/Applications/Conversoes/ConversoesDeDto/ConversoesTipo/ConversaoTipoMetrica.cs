using API.Domains;
using API.DTOs.TipoDto.TipoMetricaDto;

namespace API.Applications.Conversoes.ConversoesDeDto.ConversoesTipo
{
    public class ConversaoTipoMetrica
    {
        public static ListarTipoMetricaDto ConverterParaDto(TipoMetrica tipoMetrica)
        {
            return new ListarTipoMetricaDto
            {
                TipoMetricaId = tipoMetrica.TipoMetricaId,
                NomeMetrica = tipoMetrica.NomeMetrica
            };
        }

        public static TipoMetrica ConverterParaDomain(CriarTipoMetricaDto tipoMetricaDto)
        {
            return new TipoMetrica
            {
                NomeMetrica = tipoMetricaDto.NomeMetrica
            };
        }
    }
}