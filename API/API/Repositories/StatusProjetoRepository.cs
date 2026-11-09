using API.Contexts;
using API.Domains;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class StatusProjetoRepository : IStatusProjetoRepository
    {
        private readonly SkaL4bleContext _context;

        public StatusProjetoRepository(SkaL4bleContext context)
        {
            _context = context;
        }

        public List<StatusProjeto> ListarStatusProjeto()
        {
            return _context.StatusProjeto
                .AsNoTracking()
                .OrderBy(s => s.NomeStatusProjeto)
                .ToList();
        }

        public StatusProjeto? ObterPorId(int id)
        {
            return _context.StatusProjeto.Find(id);
        }

        public bool NomeStatusProjetoExiste(string nome)
        {
            return _context.StatusProjeto.Any(s => s.NomeStatusProjeto == nome);
        }

        public bool PossuiProjetos(int id)
        {
            return _context.StatusProjeto.Any(s =>
                s.StatusProjetoId == id && s.Projeto.Any());
        }

        public void Adicionar(StatusProjeto statusProjeto)
        {
            _context.StatusProjeto.Add(statusProjeto);
            _context.SaveChanges();
        }

        public void Atualizar(int id, StatusProjeto statusProjeto)
        {
            StatusProjeto? statusProjetoBanco = _context.StatusProjeto.Find(id);

            if (statusProjetoBanco is null)
                return;

            statusProjetoBanco.NomeStatusProjeto = statusProjeto.NomeStatusProjeto;
            _context.SaveChanges();
        }

        public void Deletar(int id)
        {
            StatusProjeto? statusProjetoBanco = _context.StatusProjeto.Find(id);

            if (statusProjetoBanco is null)
                return;

            _context.StatusProjeto.Remove(statusProjetoBanco);
            _context.SaveChanges();
        }
    }
}