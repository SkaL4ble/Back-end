using API.Domains;

namespace API.Interfaces
{
    public interface IStatusTicketRepository
    {
        List<StatusTicket> ListarStatusTicket();
        StatusTicket? ObterPorId(int id);
        bool NomeStatusTicketExiste(string nome);
        bool PossuiTickets(int id);
        void Adicionar(StatusTicket statusTicket);
        void Atualizar(int id, StatusTicket statusTicket);
        void Deletar(int id);
    }
}