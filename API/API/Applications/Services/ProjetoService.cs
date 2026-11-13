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

        public List<ListarProjetoDto> ListarProjetos()
        {
            return _repository.ListarProjetos()
                .Select(ConversaoProjeto.ConverterParaDto)
                .ToList();
        }

        public ListarProjetoDto ObterPorId(Guid id)
        {
            Projeto? projeto = BuscarOuFalhar(id);
            return ConversaoProjeto.ConverterParaDto(projeto);
        }

        public void AdicionarProjeto(CriarProjetoDto projetoDto)
        {
            projetoDto.Nome = NormalizarNome(projetoDto.Nome);

            if (_repository.NomeProjetoExiste(projetoDto.Nome))
                throw new DomainException("Projeto já existe");


            _repository.CadastrarProjeto(ConversaoProjeto.ConverterParaDomain(projetoDto));
        }

        public void AssociarUsuario(Guid projetoId, Guid usuarioId)
        {
            Projeto? projeto = BuscarOuFalhar(projetoId);
            if (projeto == null)
                throw new DomainException("Projeto não encontrado");
            if (projeto.Usuario.Any(u => u.UsuarioId == usuarioId))
                throw new DomainException("Usuário já associado ao projeto");
            _repository.AssociarUsuarioAoProjeto(projetoId, usuarioId);
        }

        public void DesassociarUsuario(Guid projetoId, Guid usuarioId)
        {
            Projeto? projeto = BuscarOuFalhar(projetoId);
            if (projeto == null)
                throw new DomainException("Projeto não encontrado");
            if (!projeto.Usuario.Any(u => u.UsuarioId == usuarioId))
                throw new DomainException("Usuário não associado ao projeto");
            _repository.DesassociarUsuarioDoProjeto(projetoId, usuarioId);
        }

        public List<ListarProjetoDto> ListarProjetosPorUsuario(Guid usuarioId)
        {
            return _repository.ListarProjetosPorUsuario(usuarioId)
                .Select(ConversaoProjeto.ConverterParaDto)
                .ToList();
        }

        public bool UsuarioEstaAssociadoAoProjeto(Guid usuarioId, Guid projetoId)
        {
            return _repository.UsuarioEstaAssociadoAoProjeto(usuarioId, projetoId);
        }
           
        public void Atualizar(Guid id, CriarProjetoDto projetoDto)
        {
            Projeto? projetoBanco = BuscarOuFalhar(id);
            string novoNome = NormalizarNome(projetoDto.Nome);

            bool nomeMudou = !string.Equals(
                projetoBanco?.NomeProjeto, novoNome, StringComparison.OrdinalIgnoreCase);

            if (nomeMudou && _repository.NomeProjetoExiste(novoNome))
                throw new DomainException("Projeto já existe");

            projetoDto.Nome = novoNome;
            _repository.AtualizarProjeto(id, ConversaoProjeto.ConverterParaDomain(projetoDto));
        }

        public void Remover(Guid id)
        {
            Projeto? projeto = BuscarOuFalhar(id);
            _repository.DeletarProjeto(projeto);
        }
    }
}
