using API.Contexts;
using API.Domains;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class TipoMetricaRepository : ITipoMetricaRepository
    {
        private readonly SkaL4bleContext _context;

        public TipoMetricaRepository(SkaL4bleContext context)
        {
            _context = context;
        }

        public List<TipoMetrica> ListarTiposMetrica()
        {
            return _context.TipoMetrica
                .AsNoTracking()
                .OrderBy(t => t.NomeMetrica)
                .ToList();
        }

        public TipoMetrica? ObterPorId(int id)
        {
            return _context.TipoMetrica.Find(id);
        }

        public bool NomeTipoMetricaExiste(string nome)
        {
            return _context.TipoMetrica.Any(t => t.NomeMetrica == nome);
        }

        public bool PossuiProjetos(int id)
        {
            return _context.TipoMetrica.Any(t =>
                t.TipoMetricaId == id && t.NomeMetrica.Any());
        }

        public void Adicionar(TipoMetrica tipoMetrica)
        {
            _context.TipoMetrica.Add(tipoMetrica);
            _context.SaveChanges();
        }

        public void Atualizar(int id, TipoMetrica tipoMetrica)
        {
            TipoMetrica? tipoMetricaBanco = _context.TipoMetrica.Find(id);

            if (tipoMetricaBanco is null)
                return;

            tipoMetricaBanco.NomeMetrica = tipoMetrica.NomeMetrica;
            _context.SaveChanges();
        }

        public void Deletar(int id)
        {
            TipoMetrica? tipoMetricaBanco = _context.TipoMetrica.Find(id);

            if (tipoMetricaBanco is null)
                return;

            _context.TipoMetrica.Remove(tipoMetricaBanco);
            _context.SaveChanges();
        }
    }
}