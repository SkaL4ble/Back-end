using API.Applications.Conversoes.ConversoesDeDto;
using API.Domains;
using API.DTOs.ProjetoDto;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services
{
    public class ProjetoService
    {
        private readonly IProjetoRepository _repository;

        public ProjetoService(IProjetoRepository repository)
        {
            _repository = repository;
        }

        public Projeto BuscarOuFalhar(Guid id)
        {
            return _repository.ObterPorId(id)
                ?? throw new DomainException("Projeto não encontrado.");
        }

        private static string NormalizarNome(string? nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("O nome do projeto é obrigatório.");

            return nome.Trim();
        }

        private static string NormalizarLinkRepositorio(string? linkRepositorio)
        {
            if (string.IsNullOrWhiteSpace(linkRepositorio))
                throw new DomainException("O link do repositório é obrigatório.");

            return linkRepositorio.Trim();
        }

        public List<ListarProjetoDto> ListarProjetos()
        {
            return _repository.ListarProjetos()
                .Select(ConversaoProjeto.ConverterParaDto)
                .ToList();
        }

        public ListarProjetoDto ObterPorId(Guid id)
        {
            Projeto projeto = BuscarOuFalhar(id);
            return ConversaoProjeto.ConverterParaDto(projeto);
        }
        public ListarProjetoDto AdicionarProjeto(CriarProjetoDto projetoDto)
        {
            projetoDto.Nome = NormalizarNome(projetoDto.Nome);
            projetoDto.LinkRepositorio = NormalizarLinkRepositorio(projetoDto.LinkRepositorio);

            if (_repository.NomeProjetoExiste(projetoDto.Nome))
                throw new DomainException("Um projeto com este nome já existe.");

            Projeto novoProjeto = ConversaoProjeto.ConverterParaDomain(projetoDto);

            if (novoProjeto.ProjetoId == Guid.Empty)
            {
                novoProjeto.ProjetoId = Guid.NewGuid();
            }

            _repository.CadastrarProjeto(novoProjeto);

            return ConversaoProjeto.ConverterParaDto(novoProjeto);
        }

        public void Atualizar(Guid id, CriarProjetoDto projetoDto)
        {
            Projeto projetoBanco = BuscarOuFalhar(id);
            string novoNome = NormalizarNome(projetoDto.Nome);

            bool nomeMudou = !string.Equals(
                projetoBanco.NomeProjeto, novoNome, StringComparison.OrdinalIgnoreCase);

            if (nomeMudou && _repository.NomeProjetoExiste(novoNome))
                throw new DomainException("Um projeto com este nome já existe.");

            projetoDto.Nome = novoNome;
            projetoDto.LinkRepositorio = NormalizarLinkRepositorio(projetoDto.LinkRepositorio);

            Projeto projetoAtualizado = ConversaoProjeto.ConverterParaDomain(projetoDto);

            projetoAtualizado.ProjetoId = id;

            _repository.AtualizarProjeto(id, projetoAtualizado);
        }

        public void Remover(Guid id)
        {
            Projeto projeto = BuscarOuFalhar(id);
            _repository.DeletarProjeto(projeto);
        }
    }
}