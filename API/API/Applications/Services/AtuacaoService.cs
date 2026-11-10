using API.Applications.Conversoes.ConversoesDeDto;
using API.Domains;
using API.DTOs.AtuacaoDto;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services
{
    public class AtuacaoService
    {
        private readonly IAtuacaoRepository _repository;

        public AtuacaoService(IAtuacaoRepository repository)
        {
            _repository = repository;
        }

        private Atuacao BuscarOuFalhar(int id)
        {
            return _repository.ObterPorId(id)
                ?? throw new DomainException("Atuação não encontrada");
        }

        private static string NormalizarNome(string? nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("O nome da atuação é obrigatório");

            return nome.Trim();
        }

        public List<ListarAtuacaoDto> ListarAtuacoes()
        {
            return _repository.ListarAtuacoes()
                .Select(ConversaoAtuacao.ConverterParaDto)
                .ToList();
        }

        public ListarAtuacaoDto ObterPorId(int id)
        {
            Atuacao atuacao = BuscarOuFalhar(id);
            return ConversaoAtuacao.ConverterParaDto(atuacao);
        }

        public void AdicionarAtuacao(CriarAtuacaoDto atuacaoDto)
        {
            if (atuacaoDto.NomeAtuacao.Length > 30 || atuacaoDto.NomeAtuacao.Length < 3)
                throw new DomainException("O nome da atuação deve ter entre 3 e 30 caracteres");

            atuacaoDto.NomeAtuacao = NormalizarNome(atuacaoDto.NomeAtuacao);

            if (_repository.NomeAtuacaoExiste(atuacaoDto.NomeAtuacao))
                throw new DomainException("Atuação já existe");

            _repository.Adicionar(ConversaoAtuacao.ConverterParaDomain(atuacaoDto));
        }

        public void Atualizar(int id, CriarAtuacaoDto atuacaoDto)
        {
            if (atuacaoDto.NomeAtuacao.Length > 30 || atuacaoDto.NomeAtuacao.Length < 3)
                throw new DomainException("O nome da atuação deve ter entre 3 e 30 caracteres");

            Atuacao atuacaoBanco = BuscarOuFalhar(id);
            string novoNome = NormalizarNome(atuacaoDto.NomeAtuacao);

            bool nomeMudou = !string.Equals(
                atuacaoBanco.NomeAtuacao, novoNome, StringComparison.OrdinalIgnoreCase);

            if (nomeMudou && _repository.NomeAtuacaoExiste(novoNome))
                throw new DomainException("Atuação já existe");

            atuacaoDto.NomeAtuacao = novoNome;
            _repository.Atualizar(id, ConversaoAtuacao.ConverterParaDomain(atuacaoDto));
        }

        public void Deletar(int id)
        {
            BuscarOuFalhar(id);

            if (_repository.PossuiUsuarios(id))
                throw new DomainException("Não é possível excluir: há usuários vinculados a esta atuação");

            _repository.Deletar(id);
        }
    }
}