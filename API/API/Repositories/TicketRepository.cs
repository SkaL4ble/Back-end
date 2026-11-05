using API.Contexts;
using API.Domains;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly SkaL4bleContext _context;

        public TicketRepository(SkaL4bleContext context)
        {
            _context = context;
        }

        // ================================= CRUD =================================
        public Ticket? ObterPorId(int id)
        {
            return _context.Ticket.Find(id);
        }

        public void AdicionarTicket(Ticket ticket)
        {
            _context.Ticket.Add(ticket);
            _context.SaveChanges();
        }

        public void AtualizarTicket(Ticket ticket)
        {
            _context.Ticket.Update(ticket);
            _context.SaveChanges();
        }

        public void DeletarTicket(Ticket ticket)
        {
            _context.Ticket.Remove(ticket);
            _context.SaveChanges();
        }

        // ================================= VALIDACOES =================================

        public Usuario? ObterUsuarioPorId(Guid usuarioId)
        {
            return _context.Usuario.Find(usuarioId);
        }


        public bool ProjetoExiste(Guid projetoId)
        {
            return _context.Projeto.Find(projetoId) is not null;
        }

        public bool TipoTicketExiste(int tipoTicketId)
        {
            return _context.TipoTicket.Find(tipoTicketId) is not null;
        }

        public bool StatusTicketExiste(int statusTicketId)
        {
            return _context.StatusTicket.Find(statusTicketId) is not null;
        }

        public bool UsuarioExiste(Guid usuarioId)
        {
            return _context.Usuario.Find(usuarioId) is not null;
        }

        // ================================ LISTAGEM =================================
        public List<Ticket> ListarTickets()
        {
            return _context.Ticket
                .Include(t => t.Projeto)
                .Include(t => t.TipoTicket)
                .Include(t => t.StatusTicket)
                .Include(t => t.Usuario)
                .AsNoTracking()
                .ToList();
        }

        public List<Usuario> ListarColaboradores(int ticketId)
        {
            return _context.Usuario
                .Where(u => u.Ticket.Any(t => t.TicketId == ticketId))
                .ToList();
        }
        public List<Ticket> ListarTicketsPorUsuario(Guid usuarioId)
        {
            return _context.Ticket
                .Where(t => t.Usuario.Any(c => c.UsuarioId == usuarioId) || t.UsuarioResponsavel == usuarioId)
                .Include(t => t.Projeto)
                .Include(t => t.TipoTicket)
                .Include(t => t.StatusTicket)
                .Include(t => t.UsuarioResponsavel)
                .Include(t => t.Usuario)
                .AsNoTracking()
                .ToList();
        }
        public List<Ticket> ListarTicketsPorProjeto(Guid projetoId)
        {
            return _context.Ticket
                .Where(t => t.ProjetoId == projetoId)
                .Include(t => t.Projeto)
                .Include(t => t.TipoTicket)
                .Include(t => t.StatusTicket)
                .Include(t => t.UsuarioResponsavel)
                .Include(t => t.Usuario)
                .AsNoTracking()
                .ToList();
        }
        public List<Ticket> ListarTicketsPorStatus(int statusTicketId)
        {
            return _context.Ticket
                .Where(t => t.StatusTicketId == statusTicketId)
                .Include(t => t.Projeto)
                .Include(t => t.TipoTicket)
                .Include(t => t.StatusTicket)
                .Include(t => t.UsuarioResponsavel)
                .Include(t => t.Usuario)
                .AsNoTracking()
                .ToList();
        }
        public List<Ticket> ListarTicketsPorTipo(int tipoTicketId)
        {
            return _context.Ticket
                .Where(t => t.TipoTicketId == tipoTicketId)
                .Include(t => t.Projeto)
                .Include(t => t.TipoTicket)
                .Include(t => t.StatusTicket)
                .Include(t => t.UsuarioResponsavel)
                .Include(t => t.Usuario)
                .AsNoTracking()
                .ToList();
        }
        public List<Ticket> ListarTicketsPorPrioridade(int prioridade)
        {
            return _context.Ticket
                .Where(t => t.Prioridade == prioridade)
                .Include(t => t.Projeto)
                .Include(t => t.TipoTicket)
                .Include(t => t.StatusTicket)
                .Include(t => t.UsuarioResponsavel)
                .Include(t => t.Usuario)
                .AsNoTracking()
                .ToList();
        }


        public List<Ticket> ListarTicketsPorPeriodo(DateTime dataInicio, DateTime dataFim)
        {
            return _context.Ticket
                .Where(t => t.DataCriacao >= dataInicio && t.DataCriacao <= dataFim)
                .Include(t => t.Projeto)
                .Include(t => t.TipoTicket)
                .Include(t => t.StatusTicket)
                .Include(t => t.UsuarioResponsavel)
                .Include(t => t.Usuario)
                .AsNoTracking()
                .ToList();
        }
        public List<Ticket> ListarTicketPorDataForaDoPrazo()
        {
            return _context.Ticket
                .Where(t => t.DataPrazo < t.DataEntrega)
                .Include(t => t.Projeto)
                .Include(t => t.TipoTicket)
                .Include(t => t.StatusTicket)
                .Include(t => t.UsuarioResponsavel)
                .Include(t => t.Usuario)
                .AsNoTracking()
                .ToList();
        }
        public List<Ticket> ListarTicketPorDataForaDoPrazoPorUsuario(Guid usuarioId)
        {
            return _context.Ticket
                .Where(t => t.DataPrazo < t.DataEntrega && (t.UsuarioResponsavel == usuarioId))
                .Include(t => t.Projeto)
                .Include(t => t.TipoTicket)
                .Include(t => t.StatusTicket)
                .Include(t => t.UsuarioResponsavel)
                .Include(t => t.Usuario)
                .AsNoTracking()
                .ToList();
        }
        public List<Ticket> ListarTicketPorDataForaDoPrazoPorProjeto(Guid projetoId)
        {
            return _context.Ticket
                .Where(t => t.DataPrazo < t.DataEntrega && (t.ProjetoId == projetoId))
                .Include(t => t.Projeto)
                .Include(t => t.TipoTicket)
                .Include(t => t.StatusTicket)
                .Include(t => t.UsuarioResponsavel)
                .Include(t => t.Usuario)
                .AsNoTracking()
                .ToList();
        }
        public List<Ticket> ListarTicketPorDataDentroDoPrazo()
        {
            return _context.Ticket
                .Where(t => t.DataPrazo >= t.DataEntrega)
                .Include(t => t.Projeto)
                .Include(t => t.TipoTicket)
                .Include(t => t.StatusTicket)
                .Include(t => t.UsuarioResponsavel)
                .Include(t => t.Usuario)
                .AsNoTracking()
                .ToList();
        }
        public List<Ticket> ListarTicketPorDataDentroDoPrazoPorUsuario(Guid usuarioId)
        {
            return _context.Ticket
                .Where(t => t.DataPrazo >= t.DataEntrega && (t.UsuarioResponsavel == usuarioId))
                .Include(t => t.Projeto)
                .Include(t => t.TipoTicket)
                .Include(t => t.StatusTicket)
                .Include(t => t.UsuarioResponsavel)
                .Include(t => t.Usuario)
                .AsNoTracking()
                .ToList();
        }
        public List<Ticket> ListarTicketPorDataDentroDoPrazoPorProjeto(Guid projetoId)
        {
            return _context.Ticket
                .Where(t => t.DataPrazo >= t.DataEntrega && (t.ProjetoId == projetoId))
                .Include(t => t.Projeto)
                .Include(t => t.TipoTicket)
                .Include(t => t.StatusTicket)
                .Include(t => t.UsuarioResponsavel)
                .Include(t => t.Usuario)
                .AsNoTracking()
                .ToList();
        }

        public List<Ticket> ListarTicketsPorUsuarioEProjeto(Guid usuarioId, Guid projetoId)
        {
            return _context.Ticket
                .Where(t => t.UsuarioResponsavel == usuarioId && t.ProjetoId == projetoId)
                .Include(t => t.Projeto)
                .Include(t => t.TipoTicket)
                .Include(t => t.StatusTicket)
                .Include(t => t.UsuarioResponsavel)
                .Include(t => t.Usuario)
                .AsNoTracking()
                .ToList();
        }

        // ================================ FUNCOES =================================
        public void MudarEstadoTicket(int ticketId, int statusTicketId)
        {
            var ticket = _context.Ticket.Find(ticketId);
            if (ticket is null)
            {
                throw new Exception("Ticket não encontrado.");
            }
            var statusTicket = _context.StatusTicket.Find(statusTicketId);
            if (statusTicket is null)
            {
                throw new Exception("Status do ticket não encontrado.");
            }
            ticket.StatusTicketId = statusTicketId;
            _context.SaveChanges();
        }

        public void AdicionarColaborador(int ticketId, Guid usuarioId)
        {
            var ticket = _context.Ticket.Find(ticketId);
            if (ticket is null)
            {
                throw new Exception("Ticket não encontrado.");
            }
            var usuario = _context.Usuario.Find(usuarioId);
            if (usuario is null)
            {
                throw new Exception("Usuário não encontrado.");
            }
            ticket.Usuario.Add(usuario);
            _context.SaveChanges();
        }

        public void RemoverColaborador(int ticketId, Guid usuarioId)
        {
            var ticket = _context.Ticket.Find(ticketId);
            if (ticket is null)
            {
                throw new Exception("Ticket não encontrado.");
            }
            var usuario = _context.Usuario.Find(usuarioId);
            if (usuario is null)
            {
                throw new Exception("Usuário não encontrado.");
            }
            ticket.Usuario.Remove(usuario);
            _context.SaveChanges();
        }

        public void TransferirResponsavel(int ticketId, Guid usuarioId)
        {
            var ticket = _context.Ticket.Find(ticketId);
            if (ticket is null)
            {
                throw new Exception("Ticket não encontrado.");
            }
            var usuario = _context.Usuario.Find(usuarioId);
            if (usuario is null)
            {
                throw new Exception("Usuário não encontrado.");
            }
            ticket.UsuarioResponsavel = usuarioId;
            _context.SaveChanges();
        }
    }
}