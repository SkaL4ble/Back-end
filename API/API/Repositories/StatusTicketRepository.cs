using API.Contexts;
using API.Domains;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class StatusTicketRepository : IStatusTicketRepository
    {
        private readonly SkaL4bleContext _context;

        public StatusTicketRepository(SkaL4bleContext context)
        {
            _context = context;
        }

        public List<StatusTicket> ListarStatusTicket()
        {
            return _context.StatusTicket
                .AsNoTracking()
                .OrderBy(s => s.NomeStatusTicket)
                .ToList();
        }

        public StatusTicket? ObterPorId(int id)
        {
            return _context.StatusTicket.Find(id);
        }

        public bool NomeStatusTicketExiste(string nome)
        {
            return _context.StatusTicket.Any(s => s.NomeStatusTicket == nome);
        }

        public bool PossuiTickets(int id)
        {
            return _context.StatusTicket.Any(s =>
                s.StatusTicketId == id && s.Ticket.Any());
        }

        public void Adicionar(StatusTicket statusTicket)
        {
            _context.StatusTicket.Add(statusTicket);
            _context.SaveChanges();
        }

        public void Atualizar(int id, StatusTicket statusTicket)
        {
            StatusTicket? statusTicketBanco = _context.StatusTicket.Find(id);

            if (statusTicketBanco is null)
                return;

            statusTicketBanco.NomeStatusTicket = statusTicket.NomeStatusTicket;
            _context.SaveChanges();
        }

        public void Deletar(int id)
        {
            StatusTicket? statusTicketBanco = _context.StatusTicket.Find(id);

            if (statusTicketBanco is null)
                return;

            _context.StatusTicket.Remove(statusTicketBanco);
            _context.SaveChanges();
        }
    }
}