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
            Usuario usuario = _context.Usuario.Find(usuarioId);
            if(usuario is null || usuario.Ativo == false)
            {
                return null;
            }
            return usuario;
        }


        public bool ProjetoExiste(Guid projetoId)
        {
            Projeto projeto = _context.Projeto.Find(projetoId);
            if(projeto is null || projeto.Ativo == false)
            {
                return false;
            }
            return true;
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
            Usuario usuario = _context.Usuario.Find(usuarioId);
            if(usuario is null || usuario.Ativo == false)
            {
                return false;
            }
            return true;
        }

        public bool ColaboradorExiste(int ticketId, Guid usuarioId)
        {
            return _context.Ticket.Any(tu => tu.TicketId == ticketId && tu.Usuario.Any(u => u.UsuarioId == usuarioId));
        }

        // ================================ LISTAGEM =================================
        public List<Ticket> ListarTickets()
        {
            return _context.Ticket
                .Include(t => t.Projeto)
                .Include(t => t.TipoTicket)
                .Include(t => t.StatusTicket)
                .Include(t => t.Usuario)
                .Where(t => t.Ativo)
                .AsNoTracking()
                .ToList();
        }

        public List<Usuario> ListarColaboradores(int ticketId)
        {
            return _context.Usuario
                .Where(u => u.Ticket.Any(t => t.TicketId == ticketId) && u.Ativo)
                .ToList();
        }
        public List<Ticket> ListarTicketsPorUsuario(Guid usuarioId)
        {
            return _context.Ticket
                .Where(t => t.Usuario.Any(c => c.UsuarioId == usuarioId) || t.UsuarioResponsavel == usuarioId && t.Ativo)
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
                .Where(t => t.ProjetoId == projetoId && t.Ativo)
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
                .Where(t => t.StatusTicketId == statusTicketId && t.Ativo)
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
                .Where(t => t.TipoTicketId == tipoTicketId && t.Ativo)
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
                .Where(t => t.Prioridade == prioridade && t.Ativo)
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
                .Where(t => t.DataCriacao >= dataInicio && t.DataCriacao <= dataFim.Date.AddDays(1) && t.Ativo)
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
                .Where(t => (t.DataPrazo < t.DataEntrega || t.DataEntrega == null)
                    && t.DataEntrega != null 
                    && t.Ativo)
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
                .Where(t => (t.DataPrazo < t.DataEntrega || t.DataEntrega == null) && (t.UsuarioResponsavel == usuarioId) && t.Ativo)
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
                .Where(t => (t.DataPrazo < t.DataEntrega || t.DataEntrega == null) && (t.ProjetoId == projetoId) && t.Ativo)
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
                .Where(t => (t.DataPrazo >= t.DataEntrega || t.DataEntrega == null) && t.DataEntrega != null && t.Ativo)
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
                .Where(t => (t.DataPrazo >= t.DataEntrega || t.DataEntrega == null) && (t.UsuarioResponsavel == usuarioId) && t.DataEntrega != null && t.Ativo)
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
                .Where(t => (t.DataPrazo >= t.DataEntrega || t.DataEntrega == null) && (t.ProjetoId == projetoId) && t.DataEntrega != null && t.Ativo)
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
                .Where(t =>( t.UsuarioResponsavel == usuarioId && t.Usuario.Any(u => u.UsuarioId == usuarioId)) && t.ProjetoId == projetoId && t.Ativo)
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
                return;
            }
            var statusTicket = _context.StatusTicket.Find(statusTicketId);
            if (statusTicket is null)
            {
                return;
            }
            else if(ticket.Ativo == false)
            {
                return;
            }
            
            ticket.StatusTicketId = statusTicketId;
            _context.SaveChanges();
        }

        public void AdicionarColaborador(int ticketId, Guid usuarioId)
        {
            var ticket = _context.Ticket.Find(ticketId);
            if (ticket is null)
            {
                return;
            }

            else if(ticket.Ativo == false)
            {
                return;
            }
            var usuario = _context.Usuario.Find(usuarioId);
            if (usuario is null)
            {
                return;
            }
            else if (usuario.Ativo == false)
            {
                return;
            }

            else if (ticket.UsuarioResponsavel == usuarioId)
            {
                return;
            }

            ticket.Usuario.Add(usuario);
            _context.SaveChanges();
        }

        public void RemoverColaborador(int ticketId, Guid usuarioId)
        {
            var ticket = _context.Ticket.Find(ticketId);
            if (ticket is null)
            {
                return;
            }
            var usuario = _context.Usuario.Find(usuarioId);
            if (usuario is null)
            {
                return;
            }

            else if(usuario.Ativo == false)
            {
                return;
            }

            ticket.Usuario.Remove(usuario);
            _context.SaveChanges();
        }

        public void TransferirResponsavel(int ticketId, Guid usuarioId)
        {
            var ticket = _context.Ticket.Find(ticketId);

            if (ticket is null)
            {
                return;
            }

            else if (ticket.Ativo == false)
            {
                return;
            }

            if (_context.Usuario.Any(u => u.UsuarioId == usuarioId && u.Projeto.Any(p => p.ProjetoId == ticket.ProjetoId && p.Ativo)))
            {
                return;
            }

            var usuario = _context.Usuario.Find(usuarioId);
            if (usuario is null)
            {
                return;
            }

            if(usuario.Ativo == false)
            {
                return;
            }

            ticket.UsuarioResponsavel = usuarioId;
            _context.SaveChanges();
        }
    }
}