using API.Applications.Conversoes.ConversoesDeDto;
using API.Domains;
using API.DTOs.ProjetoDto;
using API.DTOs.StatusTicketDto;
using API.Exceptions;
using API.Interfaces;
using API.Repositories;

namespace API.Applications.Services
{
    public class ProjetoService
    {
        private readonly IProjetoRepository _repository;

        public ProjetoService(IProjetoRepository repository)
        {
            _repository = repository;
        }

        public Projeto? BuscarOuFalhar(Guid id)
        {
            return _repository.ObterPorId(id)
                ?? throw new DomainException("Projeto não encontrado");
        }

        public static string NormalizarNome(string? nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("O nome do projeto é obrigatório");

            return nome.Trim();
        }


        public ListarProjetoDto ObterPorId(Guid id)
        {
            Projeto? projeto = BuscarOuFalhar(id);
            return ConversaoProjeto.ConverterParaDto(projeto);
        }

        public void AdicionarProjeto(CriarProjetoDto projetoDto)
        {
            projetoDto.Nome = NormalizarNome(projetoDto.Nome);


        }

    }
}
