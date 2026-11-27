
using API.Contexts;
using API.Domains;
using API.DTOs;
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

    public List<TecnologiaUsuario> ListarTecnologiasPorUsuarioId(Guid usuarioId)
    {
        return _context.TecnologiaUsuario
            .Include(x => x.Tecnologia)
            .Where(x => x.UsuarioId == usuarioId)
            .OrderByDescending(x => x.NivelTecnologia)
            .ThenByDescending(x => x.AfinidadeMedia)
            .ToList();
    }

    public TecnologiaUsuario? ObterTecnologiaDoUsuario(Guid usuarioId, int tecnologiaId)
    {
        return _context.TecnologiaUsuario
            .Include(x => x.Tecnologia)
            .FirstOrDefault(
                x =>
                    x.UsuarioId == usuarioId &&
                    x.TecnologiaId == tecnologiaId);
    }

    public List<DemandaTecnologiaDto> ListarDemandasTecnologicas()
    {
        var demandas = _context.ProjetoTecnologia
            .AsNoTracking()
            .SelectMany(
                projetoTecnologia =>
                    _context.Ticket
                        .Where(
                            ticket =>
                                ticket.Ativo &&
                                ticket.ProjetoId == projetoTecnologia.ProjetoId)
                        .Select(
                            ticket => new
                            {
                                projetoTecnologia.TecnologiaId,
                                ticket.ProjetoId
                            }))
            .GroupBy(x => x.TecnologiaId)
            .Select(
                grupo => new DemandaTecnologiaDto
                {
                    TecnologiaId = grupo.Key,

                    QuantidadeTickets = grupo.Count(),

                    QuantidadeProjetos = grupo
                        .Select(x => x.ProjetoId)
                        .Distinct()
                        .Count()
                })
            .ToList();

        var tecnologiaIds = demandas
            .Select(x => x.TecnologiaId)
            .ToList();

        var tecnologias = _context.Tecnologia
            .Where(x => tecnologiaIds.Contains(x.TecnologiaId))
            .ToDictionary(
                x => x.TecnologiaId,
                x => x.NomeTecnologia);

        foreach (var demanda in demandas)
        {
            if (tecnologias.TryGetValue( demanda.TecnologiaId, out var nomeTecnologia))
            {
                demanda.NomeTecnologia = nomeTecnologia;
            }
        }

        return demandas;
    }

    public List<TrilhaTreinamento> ListarTrilhasPorTecnologia(int tecnologiaId)
    {
        return _context.TrilhaTreinamento
            .Include(x => x.TrilhaTreinamentoTecnologia
                .Where(x => x.TecnologiaId == tecnologiaId))
            .Where(x => x.Ativa)
            .OrderBy(x => x.NivelInicial)
            .ToList();
    }

    // ================================ CADASTRO ================================

    public void AdicionarGameficacao(UsuarioGameficacao gameficacao)
    {
        _context.UsuarioGameficacao.Add(gameficacao);
    }

    public void AdicionarTecnologiaUsuario(TecnologiaUsuario tecnologiaUsuario)
    {
        _context.TecnologiaUsuario.Add(tecnologiaUsuario);
    }

    // ================================ ATUALIZAÇÃO ================================

    public void AtualizarGameficacao(UsuarioGameficacao gameficacao)
    {
        _context.UsuarioGameficacao.Update(gameficacao);
    }

    public void AtualizarTecnologiaUsuario(TecnologiaUsuario tecnologiaUsuario)
    {
        _context.TecnologiaUsuario.Update(tecnologiaUsuario);
    }

    // ================================ PERSISTÊNCIA ================================

    public void Salvar()
    {
        _context.SaveChanges();
    }
}
