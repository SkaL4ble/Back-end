using API.Contexts;
using API.Domains;
using API.Interfaces;

namespace API.Repositories
{
    public class ProjetoRepository 
    {
        private readonly SkaL4bleContext _context;

        public ProjetoRepository(SkaL4bleContext context)
        {
            _context = context;
        }
        public List<Projeto> Listar()
        {
            return _context.Projeto.ToList();
        }

        public Projeto? ObterPorNome(string nome)
        {
            return _context.Projeto.FirstOrDefault(p => p.NomeProjeto == nome);
        }

        public Projeto ObterPorId(Guid id)
        {
            return _context.Projeto.FirstOrDefault(p => p.ProjetoId == id);
        }

        public void Adicionar(Projeto projeto)
        {
            _context.Projeto.Add(projeto);
            _context.SaveChanges();
        }

        public void Atualizar(Projeto projeto, Guid id)
        {
            var projetoExistente = _context.Projeto.FirstOrDefault(p => p.ProjetoId == id);
            if (projetoExistente != null)
            {
                _context.Projeto.Update(projeto);
                _context.SaveChanges();
            }
        }
        
        public void Remover(Projeto projeto)
        {
            _context.Projeto.Remove(projeto);
            _context.SaveChanges();
        }

        public List<Projeto> ListarPorTipo(int tipoProjetoId)
        {
            return _context.Projeto.Where(p => p.TipoProjetoId == tipoProjetoId).ToList();
        }

        public List<Projeto> ListarPorStatus(int status)
        {
            return _context.Projeto.Where(p => p.StatusProjetoId == status).ToList();
        }

        public List<Projeto> ListarPorStatusETipo(int status, int tipoProjetoId)
        {
            return _context.Projeto.Where(p => p.StatusProjetoId == status && p.TipoProjetoId == tipoProjetoId).ToList();
        }

        public bool ProjetoAtivo(Guid projetoId)
        {
            var projeto = _context.Projeto.FirstOrDefault(p => p.ProjetoId == projetoId);
            if (projeto != null)
            {
                return projeto.Ativo == true;
            }
            return false;
        }
    }
}
