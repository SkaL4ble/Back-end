using API.Applications.Conversoes.ConversoesDeDto;
<<<<<<< HEAD
using API.Applications.Validacoes;
=======
using API.Applications.Services.Validacoes;
>>>>>>> 0fb51031abafc356f7cdc6125a16b23b059a7752
using API.Domains;
using API.DTOs.AtuacaoDto;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services
{
    public class AtuacaoService
    {
        private readonly IAtuacaoRepository _repository;
        private readonly ValidacoesGerais _validacoes;

        public AtuacaoService(IAtuacaoRepository repository, ValidacoesGerais validacoes)
        {
            _repository = repository;
            _validacoes = validacoes;
        }

        private Atuacao BuscarOuFalhar(int id)
        {
            return _repository.ObterPorId(id)
                ?? throw new DomainException("Atuação não encontrada");
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

<<<<<<< HEAD
            atuacaoDto.NomeAtuacao = _validacoes.NormalizarNome(atuacaoDto.NomeAtuacao);
=======
            atuacaoDto.NomeAtuacao = ValidacoesGerais.NormalizarNome(atuacaoDto.NomeAtuacao);
>>>>>>> 0fb51031abafc356f7cdc6125a16b23b059a7752

            if (_repository.NomeAtuacaoExiste(atuacaoDto.NomeAtuacao))
                throw new DomainException("Atuação já existe");

            _repository.Adicionar(ConversaoAtuacao.ConverterParaDomain(atuacaoDto));
        }

        public void Atualizar(int id, CriarAtuacaoDto atuacaoDto)
        {
            if (atuacaoDto.NomeAtuacao.Length > 30 || atuacaoDto.NomeAtuacao.Length < 3)
                throw new DomainException("O nome da atuação deve ter entre 3 e 30 caracteres");

            Atuacao atuacaoBanco = BuscarOuFalhar(id);
<<<<<<< HEAD
            string novoNome = _validacoes.NormalizarNome(atuacaoDto.NomeAtuacao);
=======
            string novoNome = ValidacoesGerais.ValidarTamanho(ValidacoesGerais.NormalizarNome(atuacaoBanco.NomeAtuacao));
>>>>>>> 0fb51031abafc356f7cdc6125a16b23b059a7752

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