using API.Contexts;
using API.Domains;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly SkaL4bleContext _context;

    public TicketRepository(SkaL4bleContext context)
    {
        _context = context;
    }

    // ================================= CRUD =================================

    public Ticket? ObterPorId(int id) =>
        _context.Ticket.FirstOrDefault(t => t.TicketId == id);

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
        // Soft delete: preserva o histórico e os vínculos do ticket.
        ticket.Ativo = false;
        _context.Ticket.Update(ticket);
        _context.SaveChanges();
    }

    // ================================ VALIDACOES ================================

    public Usuario? ObterUsuarioPorId(Guid usuarioId) =>
        _context.Usuario.FirstOrDefault(u => u.UsuarioId == usuarioId && u.Ativo);

    public bool ProjetoExiste(Guid projetoId) =>
        _context.Projeto.Any(p => p.ProjetoId == projetoId && p.Ativo);

    public bool TipoTicketExiste(int tipoTicketId) =>
        _context.TipoTicket.Any(t => t.TipoTicketId == tipoTicketId);

    public bool StatusTicketExiste(int statusTicketId) =>
        _context.StatusTicket.Any(s => s.StatusTicketId == statusTicketId);

    public bool UsuarioExiste(Guid usuarioId) =>
        _context.Usuario.Any(u => u.UsuarioId == usuarioId && u.Ativo);

    public bool ColaboradorExiste(int ticketId, Guid usuarioId) =>
        _context.Ticket.Any(t =>
            t.TicketId == ticketId &&
            t.Usuario.Any(u => u.UsuarioId == usuarioId));

    public bool UsuarioParticipaDoProjeto(Guid usuarioId, Guid projetoId) =>
        _context.Projeto.Any(p =>
            p.ProjetoId == projetoId &&
            p.Ativo &&
            p.Usuario.Any(u => u.UsuarioId == usuarioId && u.Ativo));

    // ================================ CONSULTAS =================================

    private IQueryable<Ticket> TicketsAtivos() =>
        _context.Ticket
            .Where(t => t.Ativo)
            .Include(t => t.Projeto)
            .Include(t => t.TipoTicket)
            .Include(t => t.StatusTicket)
            .Include(t => t.UsuarioResponsavelNavigation)
            .Include(t => t.Usuario)
            .AsNoTracking();

    public List<Ticket> ListarTickets() => TicketsAtivos().ToList();

    public List<Usuario> ListarColaboradores(int ticketId) =>
        _context.Usuario
            .Where(u => u.Ticket.Any(t => t.TicketId == ticketId) && u.Ativo)
            .AsNoTracking()
            .ToList();

    public List<Ticket> ListarTicketsPorUsuario(Guid usuarioId) =>
        TicketsAtivos()
            .Where(t =>
                t.UsuarioResponsavel == usuarioId ||
                t.Usuario.Any(c => c.UsuarioId == usuarioId))
            .ToList();

    public List<Ticket> ListarTicketsPorProjeto(Guid projetoId) =>
        TicketsAtivos().Where(t => t.ProjetoId == projetoId).ToList();

    public List<Ticket> ListarTicketsPorStatus(int statusTicketId) =>
        TicketsAtivos().Where(t => t.StatusTicketId == statusTicketId).ToList();

    public List<Ticket> ListarTicketsPorTipo(int tipoTicketId) =>
        TicketsAtivos().Where(t => t.TipoTicketId == tipoTicketId).ToList();

    public List<Ticket> ListarTicketsPorPrioridade(int prioridade) =>
        TicketsAtivos().Where(t => t.Prioridade == prioridade).ToList();

    public List<Ticket> ListarTicketsPorUsuarioEProjeto(Guid usuarioId, Guid projetoId) =>
        TicketsAtivos()
            .Where(t =>
                t.ProjetoId == projetoId &&
                (t.UsuarioResponsavel == usuarioId ||
                 t.Usuario.Any(u => u.UsuarioId == usuarioId)))
            .ToList();

    public List<Ticket> ListarTicketsPorPeriodo(DateTime dataInicio, DateTime dataFim) =>
        TicketsAtivos()
            .Where(t => t.DataCriacao >= dataInicio && t.DataCriacao < dataFim.Date.AddDays(1))
            .ToList();

    public List<Ticket> ListarTicketPorDataForaDoPrazo() =>
        TicketsAtivos()
            .Where(t =>
                t.DataPrazo.HasValue &&
                ((t.DataEntrega.HasValue && t.DataEntrega.Value > t.DataPrazo.Value) ||
                 (!t.DataEntrega.HasValue && t.DataPrazo.Value < DateTime.UtcNow)))
            .ToList();

    public List<Ticket> ListarTicketPorDataForaDoPrazoPorUsuario(Guid usuarioId) =>
        TicketsAtivos()
            .Where(t =>
                (t.UsuarioResponsavel == usuarioId || t.Usuario.Any(u => u.UsuarioId == usuarioId)) &&
                t.DataPrazo.HasValue &&
                ((t.DataEntrega.HasValue && t.DataEntrega.Value > t.DataPrazo.Value) ||
                 (!t.DataEntrega.HasValue && t.DataPrazo.Value < DateTime.UtcNow)))
            .ToList();

    public List<Ticket> ListarTicketPorDataForaDoPrazoPorProjeto(Guid projetoId) =>
        TicketsAtivos()
            .Where(t =>
                t.ProjetoId == projetoId &&
                t.DataPrazo.HasValue &&
                ((t.DataEntrega.HasValue && t.DataEntrega.Value > t.DataPrazo.Value) ||
                 (!t.DataEntrega.HasValue && t.DataPrazo.Value < DateTime.UtcNow)))
            .ToList();

    public List<Ticket> ListarTicketPorDataDentroDoPrazo() =>
        TicketsAtivos()
            .Where(t =>
                t.DataPrazo.HasValue &&
                ((t.DataEntrega.HasValue && t.DataEntrega.Value <= t.DataPrazo.Value) ||
                 (!t.DataEntrega.HasValue && t.DataPrazo.Value >= DateTime.UtcNow)))
            .ToList();

    public List<Ticket> ListarTicketPorDataDentroDoPrazoPorUsuario(Guid usuarioId) =>
        TicketsAtivos()
            .Where(t =>
                (t.UsuarioResponsavel == usuarioId || t.Usuario.Any(u => u.UsuarioId == usuarioId)) &&
                t.DataPrazo.HasValue &&
                ((t.DataEntrega.HasValue && t.DataEntrega.Value <= t.DataPrazo.Value) ||
                 (!t.DataEntrega.HasValue && t.DataPrazo.Value >= DateTime.UtcNow)))
            .ToList();

    public List<Ticket> ListarTicketPorDataDentroDoPrazoPorProjeto(Guid projetoId) =>
        TicketsAtivos()
            .Where(t =>
                t.ProjetoId == projetoId &&
                t.DataPrazo.HasValue &&
                ((t.DataEntrega.HasValue && t.DataEntrega.Value <= t.DataPrazo.Value) ||
                 (!t.DataEntrega.HasValue && t.DataPrazo.Value >= DateTime.UtcNow)))
            .ToList();

    // ================================ FUNCOES =================================

    public void MudarEstadoTicket(int ticketId, int statusTicketId)
    {
        var ticket = _context.Ticket.FirstOrDefault(t => t.TicketId == ticketId && t.Ativo);
        if (ticket is null)
            return;

        ticket.StatusTicketId = statusTicketId;
        _context.SaveChanges();
    }

    public void AdicionarColaborador(int ticketId, Guid usuarioId)
    {
        var ticket = _context.Ticket
            .Include(t => t.Usuario)
            .FirstOrDefault(t => t.TicketId == ticketId && t.Ativo);
        var usuario = _context.Usuario.FirstOrDefault(u => u.UsuarioId == usuarioId && u.Ativo);

        if (ticket is null || usuario is null || ticket.Usuario.Any(u => u.UsuarioId == usuarioId))
            return;

        ticket.Usuario.Add(usuario);
        _context.SaveChanges();
    }

    public void RemoverColaborador(int ticketId, Guid usuarioId)
    {
        var ticket = _context.Ticket
            .Include(t => t.Usuario)
            .FirstOrDefault(t => t.TicketId == ticketId);
        var usuario = _context.Usuario.FirstOrDefault(u => u.UsuarioId == usuarioId);

        if (ticket is null || usuario is null)
            return;

        var colaborador = ticket.Usuario.FirstOrDefault(u => u.UsuarioId == usuarioId);
        if (colaborador is null)
            return;

        ticket.Usuario.Remove(colaborador);
        _context.SaveChanges();
    }

    public void TransferirResponsavel(int ticketId, Guid usuarioId)
    {
        var ticket = _context.Ticket.FirstOrDefault(t => t.TicketId == ticketId && t.Ativo);
        if (ticket is null)
            return;

        ticket.UsuarioResponsavel = usuarioId;
        _context.SaveChanges();
    }
}
