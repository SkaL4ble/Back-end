
using API.Contexts;
using API.Domains;
using API.DTOs.GameficacaoDto;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class GameficacaoRepository : IGameficacaoRepository
{
    private readonly SkaL4bleContext _context;

    public GameficacaoRepository(SkaL4bleContext context)
    {
        _context = context;
    }

    // ================================ CONSULTAS ================================

    public UsuarioGameficacao? ObterPorUsuarioId(Guid usuarioId)
    {
        return _context.UsuarioGameficacao
            .FirstOrDefault(x => x.UsuarioId == usuarioId);
    }

    public List<TipoTicketUsuario> ListarTiposTicketPorUsuarioId(Guid usuarioId)
    {
        return _context.TipoTicketUsuario
            .Include(x => x.TipoTicket)
            .Where(x => x.UsuarioId == usuarioId)
            .OrderByDescending(x => x.NivelTipoTicket)
            .ThenByDescending(x => x.AfinidadeMedia)
            .ToList();
    }

    public TipoTicketUsuario? ObterTipoTicketDoUsuario(Guid usuarioId, int tipoTicketId)
    {
        return _context.TipoTicketUsuario
            .Include(x => x.TipoTicket)
            .FirstOrDefault(
                x =>
                    x.UsuarioId == usuarioId &&
                    x.TipoTicketId == tipoTicketId);
    }

    public List<DemandaTipoTicketDto> ListarDemandasPorTipoTicket()
    {
        return _context.Ticket
            .AsNoTracking()
            .Include(x => x.TipoTicket)
            .Where(x => x.Ativo)
            .GroupBy(
                x => new
                {
                    x.TipoTicketId,
                    x.TipoTicket.NomeTipoTicket
                })
            .Select(
                grupo => new DemandaTipoTicketDto
                {
                    TipoTicketId = grupo.Key.TipoTicketId,

                    NomeTipoTicket = grupo.Key.NomeTipoTicket,

                    QuantidadeTickets = grupo.Count(),

                    QuantidadeProjetos = grupo
                        .Select(x => x.ProjetoId)
                        .Distinct()
                        .Count()
                })
            .OrderByDescending(x => x.QuantidadeTickets)
            .ToList();
    }

    public List<TrilhaTreinamento> ListarTrilhasPorTipoTicket(int tipoTicketId)
    {
        return _context.TrilhaTreinamento
            .Include(x => x.TrilhaTreinamentoTipoTicket)
            .Where(
                x =>
                    x.Ativa &&
                    x.TrilhaTreinamentoTipoTicket
                        .Any(y => y.TipoTicketId == tipoTicketId))
            .OrderBy(x => x.NivelInicial)
            .ToList();
    }

    // ================================ CADASTRO ================================

    public void AdicionarGameficacao(UsuarioGameficacao gameficacao)
    {
        _context.UsuarioGameficacao.Add(gameficacao);
    }

    public void AdicionarTipoTicketUsuario(TipoTicketUsuario tipoTicketUsuario)
    {
        _context.TipoTicketUsuario.Add(tipoTicketUsuario);
    }

    // ================================ ATUALIZAÇÃO ================================

    public void AtualizarGameficacao(UsuarioGameficacao gameficacao)
    {
        _context.UsuarioGameficacao.Update(gameficacao);
    }

    public void AtualizarTipoTicketUsuario(TipoTicketUsuario tipoTicketUsuario)
    {
        _context.TipoTicketUsuario.Update(tipoTicketUsuario);
    }

    // ================================ PERSISTÊNCIA ================================

    public void Salvar()
    {
        _context.SaveChanges();
    }
}
