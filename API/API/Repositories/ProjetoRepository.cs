using API.Contexts;
using API.Domains;
using API.Interfaces;
namespace API.Repositories
{
    public class ProjetoRepository : IProjetoRepository
    {
        private readonly SkaL4bleContext _context;

        public ProjetoRepository(SkaL4bleContext context)
        {
            _context = context;
        }
        public List<Projeto> ListarProjetos()
        {
            return _context.Projeto.ToList();
        }

        public Projeto? ObterPorId(Guid id)
        {
            return _context.Projeto.FirstOrDefault(p => p.ProjetoId == id);
        }
        public Projeto? ObterPorNome(string nome)
        {
            return _context.Projeto.FirstOrDefault(p => p.NomeProjeto == nome);
        }
        public bool NomeProjetoExiste(string nome)
        {
            return ObterPorNome(nome) != null;
        }
        public bool LinkRepositorioExiste(string linkRepositorio)
        {
            return _context.Projeto.Any(p => p.LinkRepositorio == linkRepositorio);
        }
 
        public string? ObterLinkRepositorioPorId(Guid id)
        {
            var projeto = ObterPorId(id);
            return projeto?.LinkRepositorio;
        }
        public void CadastrarProjeto(Projeto projeto)
        {
            _context.Projeto.Add(projeto);
            _context.SaveChanges();
        }
        public void AtualizarProjeto(Guid id, Projeto projeto)
        {
            var projetoExistente = ObterPorId(id);
            if (projetoExistente != null)
            {
                _context.Entry(projetoExistente).CurrentValues.SetValues(projeto);
                _context.SaveChanges();
            }
        }

        public void DeletarProjeto(Guid id)
        {
            Projeto projeto = _context.Projeto.Find(id);

            if (projeto == null)
            {
                return;
            }
            _context.Projeto.Remove(projeto);
            _context.SaveChangesAsync();
        }
    }
}
