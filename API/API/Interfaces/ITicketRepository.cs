using API.Domains;
using API.DTOs.TicketDto;

namespace API.Interfaces
{
    public interface ITicketRepository
    {
        // ================================= CRUD =================================
        public Ticket? ObterPorId(int id);
        public void AdicionarTicket(Ticket ticket);
        public void AtualizarTicket(Ticket ticket);
        public void DeletarTicket(Ticket ticket);

        // ================================ VALIDACOES =================================
        bool ProjetoExiste(Guid projetoId);
        bool TipoTicketExiste(int tipoTicketId);
        bool StatusTicketExiste(int statusTicketId);
        bool UsuarioExiste(Guid usuarioId);
        bool ColaboradorExiste(int ticketId, Guid usuarioId);
        public Usuario? ObterUsuarioPorId(Guid usuarioId);
        bool UsuarioParticipaDoProjeto(Guid usuarioId, Guid projetoId);
        // ================================ LISTAGEM =================================
        List<Ticket> ListarTickets(FiltroTicketDto filtro);
        List<Usuario> ListarColaboradores(int ticketId);
        List<Ticket> ListarTicketsPorPeriodo(
            DateTime dataInicio,
            DateTime dataFim);
        List<Ticket> ListarTicketsForaDoPrazo(FiltroTicketForaOuDentro filtro);
        List<Ticket> ListarTicketsDentroDoPrazo(FiltroTicketForaOuDentro filtro);

        // ================================ FUNCOES =================================
        public string PegarNomeStatusTicket(int id);
        public void MudarEstadoTicket(int ticketId, int statusTicketId);
        public void AdicionarColaborador(int ticketId, Guid usuarioId);
        public void RemoverColaborador(int ticketId, Guid usuarioId);
        public void TransferirResponsavel(int ticketId, Guid usuarioId);
        public void EditarNome(int ticketId, string nome);
        public void EditarPrioridade(int ticketId, int prioridade);
        public void EditarTipo(int ticketId, int tipo);
    }
}
