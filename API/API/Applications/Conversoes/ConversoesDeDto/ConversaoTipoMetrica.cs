using API.Domains;
using API.DTOs.TipoMetricaDto;

namespace API.Applications.Conversoes.ConversoesDeDto
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