using API.Contexts;
using API.Domains;

namespace API.Repositories
{
    public class ProjetoRepository
    {
        private readonly Skal4bleContext _context;

        public ProjetoRepository(Skal4bleContext context)
        {
            _context = context;
        }
        public List<Projeto> Listar()
        {
            return _context.Projeto.ToList();
        }

        public Projeto? ObterPorNome(string nome)
        {
            return _context.Projeto.FirstOrDefault(p => p.Nome == nome);
        }

        public Projeto ObterPorId(Guid id)
        {
            return _context.Projeto.FirstOrDefault(p => p.Id == id);
        }

        public void Adicionar(Projeto projeto)
        {
            _context.Projeto.Add(projeto);
            _context.SaveChanges();
        }

        public void Atualizar(Projeto projeto)
        {
            _context.Projeto.Update(projeto);
            _context.SaveChanges();
        }
        
        public void Remover(Projeto projeto)
        {
            _context.Projeto.Remove(projeto);
            _context.SaveChanges();
        }

        public List<Projeto> ListarPorPrazo(DateTime prazo)
        {
            return _context.Projeto.Where(p => p.Prazo <= prazo).ToList();
        }

        public List<Projeto> ListarPorStatus(int status, bool s)
        {
            return _context.Projeto.Where(p => s).ToList();
        }

        public List<Projeto> ListarPorTipo(string tipo)
        {
            return _context.Projeto.Where(p => p.Tipo == tipo).ToList();
        }
    }
}
