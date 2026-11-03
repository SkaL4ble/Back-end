using API.Contexts;
using API.Domains;
using API.Interfaces;

namespace API.Repositories
{
    public class TipoMetricaRepository : ITipoMetricaRepository
    {
        private readonly SkaL4bleContext _context;
        public TipoMetricaRepository(SkaL4bleContext context)
        {
            _context = context;
        }
        public List<TipoMetrica> Listar()
        {
            return _context.TipoMetrica.ToList();
        }
        public TipoMetrica BuscarPorId(int id)
        {
            return _context.TipoMetrica.FirstOrDefault(tm => tm.TipoMetricaId == id);
        }
        public void Cadastrar(TipoMetrica tipoMetrica)
        {
            _context.TipoMetrica.Add(tipoMetrica);
            _context.SaveChanges();
        }
        public void Atualizar(int id, TipoMetrica tipoMetrica)
        {
            var tipoMetricaExistente = _context.TipoMetrica.Find(id);
            if (tipoMetricaExistente != null)
            {
                tipoMetricaExistente.NomeMetrica = tipoMetrica.NomeMetrica;
                _context.SaveChanges();
            }
        }
        public void Remover(int id)
        {
            var tipoMetrica = _context.TipoMetrica.Find(id);
            if (tipoMetrica != null)
            {
                _context.TipoMetrica.Remove(tipoMetrica);
                _context.SaveChanges();
            }
        }
    }
}