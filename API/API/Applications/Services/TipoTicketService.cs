using API.Applications.Conversoes.ConversoesDeDto;
using API.Domains;
using API.DTOs.TipoTicketDto;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services
{
    public class TipoTicketService
    {
        private readonly ITipoTicketRepository _repository;

        public TipoTicketService(ITipoTicketRepository repository)
        {
            _repository = repository;
        }

        private TipoTicket BuscarOuFalhar(int id)
        {
            return _repository.ObterPorId(id)
                ?? throw new DomainException("Tipo de ticket não encontrado");
        }

        private static string NormalizarNome(string? nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("O nome do tipo de ticket é obrigatório");

            return nome.Trim();
        }

        public List<ListarTipoTicketDto> ListarTiposTicket()
        {
            return _repository.ListarTiposTicket()
                .Select(ConversaoTipoTicket.ConverterParaDto)
                .ToList();
        }

        public ListarTipoTicketDto ObterPorId(int id)
        {
            TipoTicket tipoTicket = BuscarOuFalhar(id);
            return ConversaoTipoTicket.ConverterParaDto(tipoTicket);
        }

        public void AdicionarTipoTicket(CriarTipoTicketDto tipoTicketDto)
        {
            tipoTicketDto.NomeTipoTicket = NormalizarNome(tipoTicketDto.NomeTipoTicket);

            if (_repository.NomeTipoTicketExiste(tipoTicketDto.NomeTipoTicket))
                throw new DomainException("Tipo de ticket já existe");

            _repository.Adicionar(ConversaoTipoTicket.ConverterParaDomain(tipoTicketDto));
        }

        public void Atualizar(int id, CriarTipoTicketDto tipoTicketDto)
        {
            TipoTicket tipoTicketBanco = BuscarOuFalhar(id);
            string novoNome = NormalizarNome(tipoTicketDto.NomeTipoTicket);

            bool nomeMudou = !string.Equals(
                tipoTicketBanco.NomeTipoTicket, novoNome, StringComparison.OrdinalIgnoreCase);

            if (nomeMudou && _repository.NomeTipoTicketExiste(novoNome))
                throw new DomainException("Tipo de ticket já existe");

            tipoTicketDto.NomeTipoTicket = novoNome;
            _repository.Atualizar(id, ConversaoTipoTicket.ConverterParaDomain(tipoTicketDto));
        }

        public void Deletar(int id)
        {
            BuscarOuFalhar(id);

            if (_repository.PossuiTickets(id))
                throw new DomainException("Não é possível excluir: há tickets vinculados a este tipo de ticket");

            _repository.Deletar(id);
        }
    }
}