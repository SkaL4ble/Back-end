using API.Applications.Conversoes;
using API.Domains;
using API.DTOs.StatusProjetoDto;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services
{
    public class StatusProjetoService
    {
        private const int TamanhoMaximoNome = 30;

        private readonly IStatusProjetoRepository _repository;

        public StatusProjetoService(IStatusProjetoRepository repository)
        {
            _repository = repository;
        }

        private StatusProjeto BuscarOuFalhar(int id)
        {
            return _repository.ObterPorId(id)
                ?? throw new DomainException("Status de projeto não encontrado");
        }

        private static string NormalizarNome(string? nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("O nome do status de projeto é obrigatório");

            string nomeNormalizado = nome.Trim();

            if (nomeNormalizado.Length > TamanhoMaximoNome)
                throw new DomainException($"O nome do status de projeto deve ter no máximo {TamanhoMaximoNome} caracteres");

            return nomeNormalizado;
        }

        public List<ListarStatusProjetoDto> ListarStatusProjeto()
        {
            return _repository.ListarStatusProjeto()
                .Select(ConversaoStatusProjeto.ConverterParaDto)
                .ToList();
        }

        public ListarStatusProjetoDto ObterPorId(int id)
        {
            StatusProjeto statusProjeto = BuscarOuFalhar(id);
            return ConversaoStatusProjeto.ConverterParaDto(statusProjeto);
        }

        public void AdicionarStatusProjeto(CriarStatusProjetoDto statusProjetoDto)
        {
            statusProjetoDto.NomeStatusProjeto = NormalizarNome(statusProjetoDto.NomeStatusProjeto);

            if (_repository.NomeStatusProjetoExiste(statusProjetoDto.NomeStatusProjeto))
                throw new DomainException("Status de projeto já existe");

            _repository.Adicionar(ConversaoStatusProjeto.ConverterParaDomain(statusProjetoDto));
        }

        public void Atualizar(int id, CriarStatusProjetoDto statusProjetoDto)
        {
            StatusProjeto statusProjetoBanco = BuscarOuFalhar(id);
            string novoNome = NormalizarNome(statusProjetoDto.NomeStatusProjeto);

            bool nomeMudou = !string.Equals(
                statusProjetoBanco.NomeStatusProjeto, novoNome, StringComparison.OrdinalIgnoreCase);

            if (nomeMudou && _repository.NomeStatusProjetoExiste(novoNome))
                throw new DomainException("Status de projeto já existe");

            statusProjetoDto.NomeStatusProjeto = novoNome;
            _repository.Atualizar(id, ConversaoStatusProjeto.ConverterParaDomain(statusProjetoDto));
        }

        public void Deletar(int id)
        {
            BuscarOuFalhar(id);

            if (_repository.PossuiProjetos(id))
                throw new DomainException("Não é possível excluir: há projetos vinculados a este status de projeto");

            _repository.Deletar(id);
        }
    }
}