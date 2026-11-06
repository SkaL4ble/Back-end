using API.Applications.Conversoes.ConversoesDeDto;
using API.Domains;
using API.DTOs.StatusTicketDto;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services
{
    public class StatusTicketService
    {
        private readonly IStatusTicketRepository _repository;

        public StatusTicketService(IStatusTicketRepository repository)
        {
            _repository = repository;
        }

        private StatusTicket BuscarOuFalhar(int id)
        {
            return _repository.ObterPorId(id)
                ?? throw new DomainException("Status de ticket não encontrado");
        }

        private static string NormalizarNome(string? nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("O nome do status de ticket é obrigatório");

            return nome.Trim();
        }

        public List<ListarStatusTicketDto> ListarStatusTicket()
        {
            return _repository.ListarStatusTicket()
                .Select(ConversaoStatusTicket.ConverterParaDto)
                .ToList();
        }

        public ListarStatusTicketDto ObterPorId(int id)
        {
            StatusTicket statusTicket = BuscarOuFalhar(id);
            return ConversaoStatusTicket.ConverterParaDto(statusTicket);
        }

        public void AdicionarStatusTicket(CriarStatusTicketDto statusTicketDto)
        {
            statusTicketDto.NomeStatusTicket = NormalizarNome(statusTicketDto.NomeStatusTicket);

            if (_repository.NomeStatusTicketExiste(statusTicketDto.NomeStatusTicket))
                throw new DomainException("Status de ticket já existe");

            _repository.Adicionar(ConversaoStatusTicket.ConverterParaDomain(statusTicketDto));
        }

        public void Atualizar(int id, CriarStatusTicketDto statusTicketDto)
        {
            StatusTicket statusTicketBanco = BuscarOuFalhar(id);
            string novoNome = NormalizarNome(statusTicketDto.NomeStatusTicket);

            bool nomeMudou = !string.Equals(
                statusTicketBanco.NomeStatusTicket, novoNome, StringComparison.OrdinalIgnoreCase);

            if (nomeMudou && _repository.NomeStatusTicketExiste(novoNome))
                throw new DomainException("Status de ticket já existe");

            statusTicketDto.NomeStatusTicket = novoNome;
            _repository.Atualizar(id, ConversaoStatusTicket.ConverterParaDomain(statusTicketDto));
        }

        public void Deletar(int id)
        {
            BuscarOuFalhar(id);

            if (_repository.PossuiTickets(id))
                throw new DomainException("Não é possível excluir: há tickets vinculados a este status de ticket");

            _repository.Deletar(id);
        }
    }
}