using API.Contexts;
using API.Domains;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public sealed class GameficacaoRepository : IGameficacaoRepository
{
    private readonly SkaL4bleContext _context;

    public GameficacaoRepository(SkaL4bleContext context)
    {
        _context = context;
    }

    public async Task<FuncionarioGamificacao?>
        ObterGameficacaoAsync(
            Guid usuarioId,
            CancellationToken cancellationToken = default)
    {
        return await _context.FuncionarioGamificacao
            .FirstOrDefaultAsync(
                x => x.UsuarioId == usuarioId,
                cancellationToken);
    }

    public async Task<TecnologiaUsuario?>
        ObterTecnologiaUsuarioAsync(
            Guid usuarioId,
            int tecnologiaId,
            CancellationToken cancellationToken = default)
    {
        return await _context.TecnologiaUsuario
            .FirstOrDefaultAsync(
                x =>
                    x.UsuarioId == usuarioId &&
                    x.TecnologiaId == tecnologiaId,
                cancellationToken);
    }

    public void AdicionarGameficacao(
        FuncionarioGamificacao gamificacao)
    {
        _context.FuncionarioGamificacao.Add(gamificacao);
    }

    public void AdicionarTecnologiaUsuario(
        TecnologiaUsuario tecnologiaUsuario)
    {
        _context.TecnologiaUsuario.Add(tecnologiaUsuario);
    }

    public async Task<int> SalvarAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(
            cancellationToken);
    }
}