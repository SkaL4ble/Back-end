using API.Contexts;
using API.Domains;
using API.DTOs.TicketDto;
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

    public List<Ticket> ListarTickets(FiltroTicketDto filtro)
    {
        var query = TicketsAtivos();

        if (filtro.UsuarioId.HasValue)
        {
            var usuarioId = filtro.UsuarioId.Value;

            query = query.Where(ticket =>
                ticket.UsuarioResponsavel == usuarioId ||
                ticket.Usuario.Any(usuario =>
                    usuario.UsuarioId == usuarioId));
        }

        if (filtro.ProjetoId.HasValue)
        {
            query = query.Where(ticket =>
                ticket.ProjetoId == filtro.ProjetoId.Value);
        }

        if (filtro.StatusTicketId.HasValue)
        {
            query = query.Where(ticket =>
                ticket.StatusTicketId == filtro.StatusTicketId.Value);
        }

        if (filtro.TipoTicketId.HasValue)
        {
            query = query.Where(ticket =>
                ticket.TipoTicketId == filtro.TipoTicketId.Value);
        }

        if (filtro.Prioridade.HasValue)
        {
            query = query.Where(ticket =>
                ticket.Prioridade == filtro.Prioridade.Value);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            var nome = filtro.Nome.Trim();

            query = query.Where(ticket =>
                ticket.NomeTicket.Contains(nome));
        }

        if (filtro.DataInicio.HasValue)
        {
            query = query.Where(ticket =>
                ticket.DataCriacao >= filtro.DataInicio.Value);
        }

        if (filtro.DataFim.HasValue)
        {
            var dataFimExclusiva = filtro.DataFim.Value
                .Date
                .AddDays(1);

            query = query.Where(ticket =>
                ticket.DataCriacao < dataFimExclusiva);
        }

        return query
            .OrderByDescending(ticket => ticket.DataCriacao)
            .ToList();
    }


    public List<Usuario> ListarColaboradores(int ticketId) =>
        _context.Usuario
            .Where(u => u.Ticket.Any(t => t.TicketId == ticketId) && u.Ativo)
            .AsNoTracking()
            .ToList();


    public List<Ticket> ListarTicketsPorPeriodo(DateTime dataInicio, DateTime dataFim) =>
        TicketsAtivos()
            .Where(t => t.DataCriacao >= dataInicio && t.DataCriacao < dataFim.Date.AddDays(1))
            .ToList();

    public List<Ticket> ListarTicketsForaDoPrazo(FiltroTicketForaOuDentro filtro)
    {
        var query = TicketsAtivos()
            .Where(t => t.DataPrazo.HasValue && t.DataEntrega.HasValue && ((t.DataEntrega.HasValue && t.DataEntrega.Value > t.DataPrazo.Value) ||
                 (!t.DataEntrega.HasValue && t.DataPrazo.Value < DateTime.UtcNow)));
        if (filtro.ProjetoId != Guid.Empty)
        {
            query = query.Where(t => t.ProjetoId == filtro.ProjetoId);
        }
        else if (filtro.UsuarioId != Guid.Empty)
        {
            query = query.Where(t => t.UsuarioResponsavel == filtro.UsuarioId || t.Usuario.Any(u => u.UsuarioId == filtro.UsuarioId));
        }
        return query.ToList();
    }
    public List<Ticket> ListarTicketsDentroDoPrazo(FiltroTicketForaOuDentro filtro)
    {
        var query = TicketsAtivos()
            .Where(t => t.DataPrazo.HasValue && t.DataEntrega.HasValue && ((t.DataEntrega.HasValue && t.DataEntrega.Value <= t.DataPrazo.Value) ||
                 (!t.DataEntrega.HasValue && t.DataPrazo.Value >= DateTime.UtcNow)));
        if (filtro.ProjetoId != Guid.Empty)
        {
            query = query.Where(t => t.ProjetoId == filtro.ProjetoId);
        }
        else if (filtro.UsuarioId != Guid.Empty)
        {
            query = query.Where(t => t.UsuarioResponsavel == filtro.UsuarioId || t.Usuario.Any(u => u.UsuarioId == filtro.UsuarioId));
        }
        return query.ToList();
    }

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
