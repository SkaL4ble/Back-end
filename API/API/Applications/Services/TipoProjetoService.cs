using API.Applications.Conversoes;
using API.Domains;
using API.DTOs.TipoProjetoDto;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services
{
    public class TipoProjetoService
    {
        private readonly ITipoProjetoRepository _repository;

        public TipoProjetoService(ITipoProjetoRepository repository)
        {
            _repository = repository;
        }

        private TipoProjeto BuscarOuFalhar(int id)
        {
            return _repository.ObterPorId(id)
                ?? throw new DomainException("Tipo de projeto não encontrado");
        }

        private static string NormalizarNome(string? nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("O nome do tipo de projeto é obrigatório");

            return nome.Trim();
        }

        public List<ListarTipoProjetoDto> ListarTiposProjeto()
        {
            return _repository.ListarTiposProjeto()
                .Select(ConversaoTipoProjeto.ConverterParaDto)
                .ToList();
        }

        public ListarTipoProjetoDto ObterPorId(int id)
        {
            TipoProjeto tipoProjeto = BuscarOuFalhar(id);
            return ConversaoTipoProjeto.ConverterParaDto(tipoProjeto);
        }

        public void AdicionarTipoProjeto(CriarTipoProjetoDto tipoProjetoDto)
        {
            tipoProjetoDto.NomeTipoProjeto = NormalizarNome(tipoProjetoDto.NomeTipoProjeto);

            if (_repository.NomeTipoProjetoExiste(tipoProjetoDto.NomeTipoProjeto))
                throw new DomainException("Tipo de projeto já existe");

            _repository.Adicionar(ConversaoTipoProjeto.ConverterParaDomain(tipoProjetoDto));
        }

        public void Atualizar(int id, CriarTipoProjetoDto tipoProjetoDto)
        {
            TipoProjeto tipoProjetoBanco = BuscarOuFalhar(id);
            string novoNome = NormalizarNome(tipoProjetoDto.NomeTipoProjeto);

            bool nomeMudou = !string.Equals(
                tipoProjetoBanco.NomeTipoProjeto, novoNome, StringComparison.OrdinalIgnoreCase);

            if (nomeMudou && _repository.NomeTipoProjetoExiste(novoNome))
                throw new DomainException("Tipo de projeto já existe");

            tipoProjetoDto.NomeTipoProjeto = novoNome;
            _repository.Atualizar(id, ConversaoTipoProjeto.ConverterParaDomain(tipoProjetoDto));
        }

        public void Deletar(int id)
        {
            BuscarOuFalhar(id);

            if (_repository.PossuiProjetos(id))
                throw new DomainException("Não é possível excluir: há projetos vinculados a este tipo de projeto");

            _repository.Deletar(id);
        }
    }
}