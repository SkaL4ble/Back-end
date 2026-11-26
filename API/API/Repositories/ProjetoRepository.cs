using API.Contexts;
using API.Domains;
using API.DTOs.ProjetoDto;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace API.Repositories;

public class ProjetoRepository : IProjetoRepository
{
    private readonly SkaL4bleContext _context;

    public ProjetoRepository(SkaL4bleContext context)
    {
        _context = context;
    }

    // ================================= CRUD =================================
    public Projeto? ObterPorId(Guid id)
    {
        return _context.Projeto
            .Include(p => p.AutorNavigation)
            .Include(p => p.TipoProjeto)
            .Include(p => p.StatusProjeto)
            .Include(p => p.Usuario)
            .FirstOrDefault(p => p.ProjetoId == id && p.Ativo);
    }

    public void CadastrarProjeto(Projeto projeto)
    {
        _context.Projeto.Add(projeto);
        _context.SaveChanges();
    }

    public void AtualizarProjeto(Guid id, Projeto projeto)
    {
        var existente = _context.Projeto.FirstOrDefault(p => p.ProjetoId == id && p.Ativo)
            ?? throw new InvalidOperationException("Projeto não encontrado");

        existente.NomeProjeto = projeto.NomeProjeto;
        existente.Descricao = projeto.Descricao;
        existente.LinkRepositorio = projeto.LinkRepositorio;
        existente.StatusProjetoId = projeto.StatusProjetoId;
        existente.TipoProjetoId = projeto.TipoProjetoId;
        existente.Prazo = projeto.Prazo;

        _context.SaveChanges();
    }

    public void DeletarProjeto(Guid id)
    {
        var projeto = _context.Projeto
            .Include(p => p.Ticket)
            .FirstOrDefault(p => p.ProjetoId == id && p.Ativo)
            ?? throw new InvalidOperationException("Projeto não encontrado");

        projeto.Ativo = false;
        foreach (var ticket in projeto.Ticket)
            ticket.Ativo = false;

        _context.SaveChanges();
    }

    // ================================ LISTAGEM =================================
    public List<Projeto> ListarProjetos(FiltroProjetoDto filtro)
    {
        var query = ListarAtivos();

        if (filtro.UsuarioId.HasValue)
            query = query.Where(p => p.Autor == filtro.UsuarioId.Value);

        if (filtro.TipoId.HasValue)
            query = query.Where(p => p.TipoProjetoId == filtro.TipoId.Value);

        if (filtro.StatusId.HasValue)
            query = query.Where(p => p.StatusProjetoId == filtro.StatusId.Value);

        return query.OrderByDescending(p => p.DataCriacao).ToList();
    }

    public List<Projeto> ListarProjetosForaDoPrazo(FiltroProjetoDto filtro)
    {
        var query = AplicarFiltrosPrazo(filtro);
        return query
            .Where(p => p.Prazo.HasValue && p.Prazo.Value < DateTime.UtcNow)
            .OrderByDescending(p => p.DataCriacao)
            .ToList();
    }

    public List<Projeto> ListarProjetosDentroDoPrazo(FiltroProjetoDto filtro)
    {
        var query = AplicarFiltrosPrazo(filtro);
        return query
            .Where(p => !p.Prazo.HasValue || p.Prazo.Value >= DateTime.UtcNow)
            .OrderByDescending(p => p.DataCriacao)
            .ToList();
    }

    private IQueryable<Projeto> AplicarFiltrosPrazo(FiltroProjetoDto filtro)
    {
        var query = ListarAtivos();

        if (filtro.UsuarioId.HasValue)
            query = query.Where(p => p.Autor == filtro.UsuarioId.Value);

        if (filtro.TipoId.HasValue)
            query = query.Where(p => p.TipoProjetoId == filtro.TipoId.Value);

        if (filtro.StatusId.HasValue)
            query = query.Where(p => p.StatusProjetoId == filtro.StatusId.Value);

        return query;
    }

    private IQueryable<Projeto> ListarAtivos()
    {
        return _context.Projeto
            .Include(p => p.AutorNavigation)
            .Include(p => p.TipoProjeto)
            .Include(p => p.StatusProjeto)
            .Include(p => p.Usuario)
            .Where(p => p.Ativo)
            .AsNoTracking();
    }

    // ================================ VALIDAÇÕES =================================

    public bool UsuarioExiste(Guid id)
    {
        return _context.Usuario.Any(u => u.UsuarioId == id);
    }
    public bool NomeProjetoExiste(string nome, Guid? idIgnorado = null)
    {
        return _context.Projeto.Any(p =>
            p.Ativo &&
            p.NomeProjeto == nome &&
            (!idIgnorado.HasValue || p.ProjetoId != idIgnorado.Value));
    }

    public bool LinkRepositorioExiste(string linkRepositorio, Guid? idIgnorado = null)
    {
        return _context.Projeto.Any(p =>
            p.Ativo &&
            p.LinkRepositorio == linkRepositorio &&
            (!idIgnorado.HasValue || p.ProjetoId != idIgnorado.Value));
    }

    public void AtualizarColaboradores(Guid projetoId, IEnumerable<Guid> usuariosIds)
    {
        var projeto = _context.Projeto
            .Include(p => p.Usuario)
            .FirstOrDefault(p => p.ProjetoId == projetoId && p.Ativo)
            ?? throw new InvalidOperationException("Projeto não encontrado");

        var ids = usuariosIds.Distinct().ToList();
        var usuarios = _context.Usuario
            .Where(u => ids.Contains(u.UsuarioId) && u.Ativo)
            .ToList();

        if (usuarios.Count != ids.Count)
            throw new InvalidOperationException("Um dos usuários informados não foi encontrado ou está inativo");

        projeto.Usuario.Clear();
        foreach (var usuario in usuarios)
            projeto.Usuario.Add(usuario);

        _context.SaveChanges();
    }

    public bool ProjetoPossuiColaboradores(Guid projetoId)
    {
        return _context.Projeto
            .Where(p => p.ProjetoId == projetoId)
            .SelectMany(p => p.Usuario)
            .Any();
    }

    public bool UsuarioEstaAssociadoAoProjetoPorNome(Guid usuarioId, string nomeProjeto)
    {
        return _context.Projeto.Any(p => p.Autor == usuarioId && p.NomeProjeto == nomeProjeto && p.Ativo);
    }

    public bool UsuarioEstaAssociadoAoProjeto(Guid usuarioId, Guid projetoId)
    {
        return _context.Projeto.Any(p => p.ProjetoId == projetoId && p.Ativo &&
            p.Usuario.Any(u => u.UsuarioId == usuarioId));
    }

    public bool UsuarioEstaAssociadoAoProjetoPorId(Guid usuarioId, Guid projetoId)
    {
        return UsuarioEstaAssociadoAoProjeto(usuarioId, projetoId);
    }

    // ================================ FUNCOES =================================
    public string? ObterLinkRepositorioPorId(Guid id)
    {
        return _context.Projeto
            .Where(p => p.ProjetoId == id && p.Ativo)
            .Select(p => p.LinkRepositorio)
            .FirstOrDefault();
    }

    public void AssociarUsuarioAoProjeto(Guid usuarioId, Guid projetoId)
    {
        var projeto = _context.Projeto
            .Include(p => p.Usuario)
            .FirstOrDefault(p => p.ProjetoId == projetoId && p.Ativo)
            ?? throw new InvalidOperationException("Projeto não encontrado");
        var usuario = _context.Usuario.FirstOrDefault(u => u.UsuarioId == usuarioId && u.Ativo)
            ?? throw new InvalidOperationException("Usuário não encontrado ou inativo");

        if (!projeto.Usuario.Any(u => u.UsuarioId == usuarioId))
            projeto.Usuario.Add(usuario);

        _context.SaveChanges();
    }

    public void DesassociarUsuarioDoProjeto(Guid usuarioId, Guid projetoId)
    {
        var projeto = _context.Projeto
            .Include(p => p.Usuario)
            .FirstOrDefault(p => p.ProjetoId == projetoId && p.Ativo)
            ?? throw new InvalidOperationException("Projeto não encontrado");
        var usuario = projeto.Usuario.FirstOrDefault(u => u.UsuarioId == usuarioId);

        if (usuario != null)
        {
            projeto.Usuario.Remove(usuario);
            _context.SaveChanges();
        }
    }

    public async Task<IDbContextTransaction> ObterTransacaoBanco()
    {
        return await _context.Database.BeginTransactionAsync();
    }
}
