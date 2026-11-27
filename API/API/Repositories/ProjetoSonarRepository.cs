using API.Contexts;
using API.Domains;
using API.Interfaces;

namespace API.Repositories;

public class ProjetoSonarRepository : IProjetoSonarRepository
{
    private readonly SkaL4bleContext _context;

    public ProjetoSonarRepository(SkaL4bleContext context)
    {
        _context = context;
    }

    public ProjetoSonar? ObterPorId(Guid projetoId) =>
    _context.ProjetoSonar.FirstOrDefault(p => p.ProjetoId == projetoId);

    public bool WorkflowExiste(int workflowId) =>
    _context.Workflow.Any(w =>
        w.WorkflowId == workflowId);

    public List<Workflow> ObterWorkflows() => _context.Workflow.ToList();

    public bool ProjetoExiste(Guid projetoId) =>
        _context.Projeto.Any(p => p.ProjetoId == projetoId && p.Ativo);

    public void Adicionar(ProjetoSonar projetoSonar)
    {
        _context.ProjetoSonar.Add(projetoSonar);
        _context.SaveChanges();
    }

    public void Atualizar(ProjetoSonar projetoSonar)
    {
        _context.ProjetoSonar.Update(projetoSonar);
        _context.SaveChanges();
    }

    public List<Guid> ListarIdsPorUsuario(Guid usuarioId)
    {
        return _context.Projeto
            .Where(projeto =>
                projeto.Ativo &&
                projeto.Usuario.Any(usuario =>
                    usuario.UsuarioId == usuarioId))
            .Select(projeto => projeto.ProjetoId)
            .ToList();
    }

    public List<string> ListarChavesSonarPorProjeto(
    Guid projetoId)
    {
        return _context.ProjetoSonar
            .Where(projetoSonar =>
                projetoSonar.ProjetoId == projetoId)
            .Select(projetoSonar =>
                projetoSonar.ChaveSonar)
            .Where(chave =>
                !string.IsNullOrWhiteSpace(chave))
            .ToList();
    }

}