using API.Contexts;
using API.Domains;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class TipoTicketRepository : ITipoTicketRepository
    {
        private readonly SkaL4bleContext _context;

        public TipoTicketRepository(SkaL4bleContext context)
        {
            _context = context;
        }

        public List<TipoTicket> ListarTiposTicket()
        {
            return _context.TipoTicket
                .AsNoTracking()
                .OrderBy(t => t.NomeTipoTicket)
                .ToList();
        }

        public TipoTicket? ObterPorId(int id)
        {
            return _context.TipoTicket.Find(id);
        }

        public bool NomeTipoTicketExiste(string nome)
        {
            return _context.TipoTicket.Any(t => t.NomeTipoTicket == nome);
        }

        public bool PossuiTickets(int id)
        {
            return _context.TipoTicket.Any(t =>
                t.TipoTicketId == id && t.Ticket.Any());
        }

        public void Adicionar(TipoTicket tipoTicket)
        {
            _context.TipoTicket.Add(tipoTicket);
            _context.SaveChanges();
        }

        public void Atualizar(int id, TipoTicket tipoTicket)
        {
            TipoTicket? tipoTicketBanco = _context.TipoTicket.Find(id);

            if (tipoTicketBanco is null)
                return;

            tipoTicketBanco.NomeTipoTicket = tipoTicket.NomeTipoTicket;
            _context.SaveChanges();
        }

        public void Deletar(int id)
        {
            TipoTicket? tipoTicketBanco = _context.TipoTicket.Find(id);

            if (tipoTicketBanco is null)
                return;

            _context.TipoTicket.Remove(tipoTicketBanco);
            _context.SaveChanges();
        }
    }
}