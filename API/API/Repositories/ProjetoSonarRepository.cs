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

    public ProjetoSonar? ObterPorId(Guid projetoSonarId) =>
        _context.ProjetoSonar.FirstOrDefault(p => p.ProjetoSonarId == projetoSonarId);

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
}