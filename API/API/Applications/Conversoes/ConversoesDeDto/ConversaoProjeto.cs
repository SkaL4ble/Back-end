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
                Nome = projeto.NomeProjeto,
                Status = projeto.StatusProjetoId,
                Prazo = projeto.Prazo
            };
        }

        public static Projeto ConverterParaDomain(CriarProjetoDto projetoDto)
        {
            return new Projeto
            {
                NomeProjeto = projetoDto.Nome,
                Descricao = projetoDto.Descricao,
                LinkRepositorio = projetoDto.LinkRepositorio,
                Autor = projetoDto.Autor,
                StatusProjetoId = projetoDto.StatusId,
                TipoProjetoId = projetoDto.TipoId,
                Prazo = projetoDto.Prazo,
                DataCriacao = projetoDto.DataCriacao
            };
        }
    }
}