using API.Domains;

namespace API.Interfaces
{
    public interface ITicketRepository
    {
        // ================================= CRUD =================================
        public Ticket ObterPorId(int id);
        public void AdicionarTicket(Ticket ticket);
        public void AtualizarTicket(Ticket ticket);
        public void DeletarTicket(Ticket ticket);

        // ================================ VALIDACOES =================================
        bool ProjetoExiste(Guid projetoId);
        bool TipoTicketExiste(int tipoTicketId);
        bool StatusTicketExiste(int statusTicketId);
        bool UsuarioExiste(Guid usuarioId);
        public Usuario? ObterUsuarioPorId(Guid usuarioId);
        // ================================ LISTAGEM =================================
        public List<Ticket> ListarTickets();
        public List<Usuario> ListarColaboradores(int ticketId);
        public List<Ticket> ListarTicketsPorUsuario(Guid usuarioId);
        public List<Ticket> ListarTicketsPorProjeto(Guid projetoId);
        public List<Ticket> ListarTicketsPorStatus(int statusTicketId);
        public List<Ticket> ListarTicketsPorTipo(int tipoTicketId);
        public List<Ticket> ListarTicketsPorPrioridade(int prioridade);
        public List<Ticket> ListarTicketsPorUsuarioEProjeto(Guid usuarioId, Guid projetoId);


        public List<Ticket> ListarTicketsPorPeriodo(DateTime dataInicio, DateTime dataFim);
        public List<Ticket> ListarTicketPorDataForaDoPrazo();
        public List<Ticket> ListarTicketPorDataForaDoPrazoPorUsuario(Guid usuarioId);
        public List<Ticket> ListarTicketPorDataForaDoPrazoPorProjeto(Guid projetoId);
        public List<Ticket> ListarTicketPorDataDentroDoPrazo();
        public List<Ticket> ListarTicketPorDataDentroDoPrazoPorUsuario(Guid usuarioId);
        public List<Ticket> ListarTicketPorDataDentroDoPrazoPorProjeto(Guid projetoId);

        // ================================ FUNCOES =================================
        public void MudarEstadoTicket(int ticketId, int statusTicketId);
        public void AdicionarColaborador(int ticketId, Guid usuarioId);
        public void RemoverColaborador(int ticketId, Guid usuarioId);
        public void TransferirResponsavel(int ticketId, Guid usuarioId);
    }
}
