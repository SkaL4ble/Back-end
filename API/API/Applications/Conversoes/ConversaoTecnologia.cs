using API.Domains;
using API.DTOs.TecnologiaDto;

namespace API.Applications.Conversoes
{
    public class ConversaoTecnologia
    {
        public static ListarTecnologiaDto ConverterParaDto(Tecnologia tecnologia)
        {
            return new ListarTecnologiaDto
            {
                TecnologiaId = tecnologia.TecnologiaId,
                NomeTecnologia = tecnologia.NomeTecnologia
            };
        }

        public static Tecnologia ConverterParaDomain(CriarTecnologiaDto tecnologiaDto)
        {
            return new Tecnologia
            {
                NomeTecnologia = tecnologiaDto.NomeTecnologia
            };
        }
    }
}