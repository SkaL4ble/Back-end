using API.Domains;
using API.DTOs.AtuacaoDto;

namespace API.Applications.Conversoes
{
    public class ConversaoAtuacao
    {
        public static ListarAtuacaoDto ConverterParaDto(Atuacao atuacao)
        {
            return new ListarAtuacaoDto
            {
                AtuacaoId = atuacao.AtuacaoId,
                NomeAtuacao = atuacao.NomeAtuacao
            };
        }

        public static Atuacao ConverterParaDomain(CriarAtuacaoDto atuacaoDto)
        {
            return new Atuacao
            {
                NomeAtuacao = atuacaoDto.NomeAtuacao
            };
        }
    }
}