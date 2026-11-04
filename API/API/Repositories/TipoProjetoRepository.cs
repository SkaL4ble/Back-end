using API.Contexts;
using API.Domains;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class TipoProjetoRepository : ITipoProjetoRepository
    {
        private readonly SkaL4bleContext _context;

        public TipoProjetoRepository(SkaL4bleContext context)
        {
            _context = context;
        }

        public List<TipoProjeto> ListarTiposProjeto()
        {
            return _context.TipoProjeto
                .AsNoTracking()
                .OrderBy(t => t.NomeTipoProjeto)
                .ToList();
        }

        public TipoProjeto? ObterPorId(int id)
        {
            return _context.TipoProjeto.Find(id);
        }

        public bool NomeTipoProjetoExiste(string nome)
        {
            return _context.TipoProjeto.Any(t => t.NomeTipoProjeto == nome);
        }

        public bool PossuiProjetos(int id)
        {
            return _context.TipoProjeto.Any(t =>
                t.TipoProjetoId == id && t.Projeto.Any());
        }

        public void Adicionar(TipoProjeto tipoProjeto)
        {
            _context.TipoProjeto.Add(tipoProjeto);
            _context.SaveChanges();
        }

        public void Atualizar(int id, TipoProjeto tipoProjeto)
        {
            TipoProjeto? tipoProjetoBanco = _context.TipoProjeto.Find(id);

            if (tipoProjetoBanco is null)
                return;

            tipoProjetoBanco.NomeTipoProjeto = tipoProjeto.NomeTipoProjeto;
            _context.SaveChanges();
        }

        public void Deletar(int id)
        {
            TipoProjeto? tipoProjetoBanco = _context.TipoProjeto.Find(id);

            if (tipoProjetoBanco is null)
                return;

            _context.TipoProjeto.Remove(tipoProjetoBanco);
            _context.SaveChanges();
        }
    }
}