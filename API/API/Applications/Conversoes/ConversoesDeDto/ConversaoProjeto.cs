using API.Domains;
using API.DTOs.ProjetoDto;

namespace API.Applications.Conversoes.ConversoesDeDto
{
    public class ConversaoProjeto
    {
        public static ListarProjetoDto ConverterParaDto(Projeto projeto)
        {
            return new ListarProjetoDto
            {
                Id = projeto.ProjetoId,
                Nome = projeto.NomeProjeto
            };
        }

        public static Projeto ConverterParaDomain(CriarProjetoDto projetoDto)
        {
            return new Projeto
            {
                NomeProjeto = projetoDto.Nome
            };
        }
    }
}