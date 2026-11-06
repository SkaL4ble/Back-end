using API.Applications.Conversoes;
using API.Domains;
using API.DTOs.TicketDto;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services
{
    public class TicketService
    {
        private readonly ITicketRepository _repository;

        public TicketService(ITicketRepository repository)
        {
            _repository = repository;
        }

        //=============================== VALIDACOES =================================

        private Ticket BuscarOuFalhar(int id)
        {
            return _repository.ObterPorId(id)
                ?? throw new DomainException("Ticket não encontrado");
        }

        private static string NormalizarNome(string? nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("O nome do ticket é obrigatório");
            else if (nome.Length < 3)
                throw new DomainException("O nome do ticket não pode ter menos de 3 caracteres");
            else if(nome.Length > 125)
                throw new DomainException("O nome do ticket não pode ter mais de 125 caracteres");

            return nome.Trim();
        }

        private void ValidarUsuario(Guid usuarioId)
        {
            if (!_repository.UsuarioExiste(usuarioId))
                throw new DomainException("Usuário não encontrado");
        }

        private void ValidarProjeto(Guid projetoId)
        {
            if (!_repository.ProjetoExiste(projetoId))
                throw new DomainException("Projeto não encontrado");
        }

        private void ValidarTipoTicket(int tipoTicketId)
        {
            if (!_repository.TipoTicketExiste(tipoTicketId))
                throw new DomainException("Tipo de ticket não encontrado");
        }

        private void ValidarStatusTicket(int statusTicketId)
        {
            if (!_repository.StatusTicketExiste(statusTicketId))
                throw new DomainException("Status de ticket não encontrado");
        }

        private void ValidarRelacionamentos(
            Guid projetoId, int tipoTicketId, int statusTicketId, Guid? usuarioResponsavel)
        {
            ValidarProjeto(projetoId);
            ValidarTipoTicket(tipoTicketId);
            ValidarStatusTicket(statusTicketId);

            if (usuarioResponsavel.HasValue)
                ValidarUsuario(usuarioResponsavel.Value);
        }

        private static List<ListarTicketDto> ParaDtos(IEnumerable<Ticket> tickets)
        {
            return tickets.Select(ConversaoTicket.ConverterParaDto).ToList();
        }

        //================================ CRUD =================================

        public ListarTicketDto ObterPorId(int id)
        {
            Ticket ticket = BuscarOuFalhar(id);
            return ConversaoTicket.ConverterParaDto(ticket);
        }

        public void AdicionarTicket(CriarTicketDto ticketDto)
        {
            ticketDto.NomeTicket = NormalizarNome(ticketDto.NomeTicket);

            if(ticketDto.Prioridade < 1)
                throw new DomainException("A prioridade do ticket é obrigatória");

            ValidarRelacionamentos(
                ticketDto.ProjetoId,
                ticketDto.TipoTicketId,
                ticketDto.StatusTicketId,
                ticketDto.UsuarioResponsavel);

            _repository.AdicionarTicket(ConversaoTicket.ConverterParaDomain(ticketDto));
        }

        public void Atualizar(int id, AtualizarTicketDto ticketDto)
        {
            Ticket ticketBanco = BuscarOuFalhar(id);
            ticketDto.NomeTicket = NormalizarNome(ticketDto.NomeTicket);

            ValidarRelacionamentos(
                ticketDto.ProjetoId,
                ticketDto.TipoTicketId,
                ticketDto.StatusTicketId,
                ticketDto.UsuarioResponsavel);

            ConversaoTicket.AplicarAtualizacao(ticketBanco, ticketDto);
            _repository.AtualizarTicket(ticketBanco);
        }

        public void Deletar(int id)
        {
            Ticket ticketBanco = BuscarOuFalhar(id);
            _repository.DeletarTicket(ticketBanco);
        }

        //================================ FUNCOES =================================

        public void MudarEstadoTicket(int ticketId, int statusTicketId)
        {
            Ticket ticket = BuscarOuFalhar(ticketId);
            ValidarStatusTicket(statusTicketId);

            if (ticket.StatusTicketId == statusTicketId)
                throw new DomainException("O ticket já está com este status");

            _repository.MudarEstadoTicket(ticketId, statusTicketId);
        }

        public void TransferirResponsavel(int ticketId, Guid usuarioId)
        {
            Ticket ticket = BuscarOuFalhar(ticketId);
            ValidarUsuario(usuarioId);

            if (ticket.UsuarioResponsavel == usuarioId)
                throw new DomainException("Este usuário já é o responsável pelo ticket");

            _repository.TransferirResponsavel(ticketId, usuarioId);
        }

        public void AdicionarColaborador(int ticketId, Guid usuarioId)
        {
            BuscarOuFalhar(ticketId);
            ValidarUsuario(usuarioId);

            if(_repository.ColaboradorExiste(ticketId, usuarioId))
                throw new DomainException("Este usuário já é colaborador do ticket");

            Usuario usuarioBanco = _repository.ObterUsuarioPorId(usuarioId)
                ?? throw new DomainException("Usuário não encontrado");

            if(_repository.ObterPorId(ticketId)?.Usuario.Any(c => c.UsuarioId == usuarioId) == true)
                throw new DomainException("Este usuário já é colaborador do ticket");

            _repository.AdicionarColaborador(ticketId, usuarioId);
        }

        public void RemoverColaborador(int ticketId, Guid usuarioId)
        {
            BuscarOuFalhar(ticketId);
            ValidarUsuario(usuarioId);

            if (_repository.ColaboradorExiste(ticketId, usuarioId))
                throw new DomainException("Este usuário já é colaborador do ticket");

            if (_repository.ObterPorId(ticketId)?.Usuario.Any(c => c.UsuarioId == usuarioId) == false)
                throw new DomainException("Este usuário não é colaborador deste ticket");


            _repository.RemoverColaborador(ticketId, usuarioId);
        }

        //================================ LISTAGEM =================================

        public List<ListarTicketDto> ListarTickets()
        {
            return ParaDtos(_repository.ListarTickets());
        }

        public List<ListarTicketDto> ListarTicketsPorUsuario(Guid usuarioId)
        {
            ValidarUsuario(usuarioId);
            return ParaDtos(_repository.ListarTicketsPorUsuario(usuarioId));
        }

        public List<ListarTicketDto> ListarTicketsPorProjeto(Guid projetoId)
        {
            ValidarProjeto(projetoId);
            return ParaDtos(_repository.ListarTicketsPorProjeto(projetoId));
        }

        public List<ListarTicketDto> ListarTicketsPorStatus(int statusTicketId)
        {
            ValidarStatusTicket(statusTicketId);
            return ParaDtos(_repository.ListarTicketsPorStatus(statusTicketId));
        }

        public List<ListarTicketDto> ListarTicketsPorTipo(int tipoTicketId)
        {
            ValidarTipoTicket(tipoTicketId);
            return ParaDtos(_repository.ListarTicketsPorTipo(tipoTicketId));
        }

        public List<ListarTicketDto> ListarTicketsPorUsuarioEProjeto(Guid usuarioId, Guid projetoId)
        {
            ValidarUsuario(usuarioId);
            ValidarProjeto(projetoId);
            return ParaDtos(_repository.ListarTicketsPorUsuarioEProjeto(usuarioId, projetoId));
        }

        public List<ListarTicketDto> ListarTicketsPorPrioridade(int prioridade)
        {
            return ParaDtos(_repository.ListarTicketsPorPrioridade(prioridade));
        }

        public List<ListarTicketDto> ListarTicketsPorPeriodo(DateTime dataInicio, DateTime dataFim)
        {
            if (dataInicio > dataFim)
                throw new DomainException("A data de início não pode ser maior que a data de fim");

            return ParaDtos(_repository.ListarTicketsPorPeriodo(dataInicio, dataFim));
        }

        //================================ PRAZO =================================

        public List<ListarTicketDto> ListarTicketPorDataForaDoPrazo()
        {
            return ParaDtos(_repository.ListarTicketPorDataForaDoPrazo());
        }

        public List<ListarTicketDto> ListarTicketPorDataForaDoPrazoPorUsuario(Guid usuarioId)
        {
            ValidarUsuario(usuarioId);
            return ParaDtos(_repository.ListarTicketPorDataForaDoPrazoPorUsuario(usuarioId));
        }

        public List<ListarTicketDto> ListarTicketPorDataForaDoPrazoPorProjeto(Guid projetoId)
        {
            ValidarProjeto(projetoId);
            return ParaDtos(_repository.ListarTicketPorDataForaDoPrazoPorProjeto(projetoId));
        }

        public List<ListarTicketDto> ListarTicketPorDataDentroDoPrazo()
        {
            return ParaDtos(_repository.ListarTicketPorDataDentroDoPrazo());
        }

        public List<ListarTicketDto> ListarTicketPorDataDentroDoPrazoPorUsuario(Guid usuarioId)
        {
            ValidarUsuario(usuarioId);
            return ParaDtos(_repository.ListarTicketPorDataDentroDoPrazoPorUsuario(usuarioId));
        }

        public List<ListarTicketDto> ListarTicketPorDataDentroDoPrazoPorProjeto(Guid projetoId)
        {
            ValidarProjeto(projetoId);
            return ParaDtos(_repository.ListarTicketPorDataDentroDoPrazoPorProjeto(projetoId));
        }
    }
}