using API.Domains;

namespace API.Interfaces
{
    public interface ITipoTicketRepository
    {
        List<TipoTicket> ListarTiposTicket();
        TipoTicket? ObterPorId(int id);
        bool NomeTipoTicketExiste(string nome);
        bool PossuiTickets(int id);
        void Adicionar(TipoTicket tipoTicket);
        void Atualizar(int id, TipoTicket tipoTicket);
        void Deletar(int id);
    }
}