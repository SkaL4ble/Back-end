using API.Contexts;
using API.Domains;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace API.Repositories
{
    public class ProjetoRepository : IProjetoRepository
    {
        private readonly SkaL4bleContext _context;

        public ProjetoRepository(SkaL4bleContext context)
        {
            _context = context;
        }
        //================================= CRUD ==========================================
        public Projeto? ObterPorId(Guid id)
        {
            return _context.Projeto.Find(id);
        }
        public void CadastrarProjeto(Projeto projeto)
        {
            _context.Projeto.Add(projeto);
            _context.SaveChanges();
        }
        public void AtualizarProjeto(Guid id, Projeto projeto)
        {
            var projetoExistente = _context.Projeto.Find(id);
            if (projetoExistente != null)
            {
                _context.Entry(projetoExistente).CurrentValues.SetValues(projeto);
                _context.SaveChanges();
            }
        }
        public void DeletarProjeto(Guid id)
        {
            var projeto = _context.Projeto.Find(id);
            if (projeto != null)
            {
                _context.Projeto.Remove(projeto);
                _context.SaveChanges();
            }
        }

        //================================= LISTAGENS ==========================================
        public List<Projeto> ListarProjetos()
        {
            return _context.Projeto
                .Include(u => u.AutorNavigation)
                .Include(t => t.TipoProjeto)
                .Include(s => s.StatusProjeto)
                .ToList();
        }
        public List<Projeto> ListarProjetosPorUsuario(Guid usuarioId)
        {
            return _context.Projeto
                .Where(p => p.Autor == usuarioId)
                .Include(u => u.AutorNavigation)
                .Include(t => t.TipoProjeto)
                .Include(s => s.StatusProjeto)
                .ToList();
        }
        public List<Projeto> ListarProjetosPorStatus(int statusId)
        {
            return _context.Projeto
                .Where(p => p.StatusProjetoId == statusId)
                .Include(u => u.AutorNavigation)
                .Include(t => t.TipoProjeto)
                .Include(s => s.StatusProjeto)
                .ToList();
        }
        public List<Projeto> ListarProjetosPorTipo(int tipoId)
        {
            return _context.Projeto
                .Where(p => p.TipoProjetoId == tipoId)
                .Include(u => u.AutorNavigation)
                .Include(t => t.TipoProjeto)
                .Include(s => s.StatusProjeto)
                .ToList();
        }
        public List<Projeto> ListarProjetosPorStatusETipo(int statusId, int tipoId)
        {
            return _context.Projeto
                .Where(p => p.StatusProjetoId == statusId && p.TipoProjetoId == tipoId)
                .Include(u => u.AutorNavigation)
                .Include(t => t.TipoProjeto)
                .Include(s => s.StatusProjeto)
                .ToList();
        }
        public Projeto? ObterPorNome(string nome)
        {
            return _context.Projeto.FirstOrDefault(p => p.NomeProjeto == nome);
        }

        // ================================= VALIDAÇÕES ==========================================
        public bool NomeProjetoExiste(string nome)
        {
            return _context.Projeto.Any(p => p.NomeProjeto == nome);
        }
        public bool LinkRepositorioExiste(string linkRepositorio)
        {
            return _context.Projeto.Any(p => p.LinkRepositorio == linkRepositorio);
        }
        public bool UsuarioExiste(Guid usuarioId)
        {
            return _context.Usuario.Any(u => u.UsuarioId == usuarioId);
        }
        public bool StatusExiste(int statusId)
        {
            return _context.StatusProjeto.Any(s => s.StatusProjetoId == statusId);
        }
        public bool TipoExiste(int tipoId)
        {
            return _context.TipoProjeto.Any(t => t.TipoProjetoId == tipoId);
        }
        public bool UsuarioEstaAssociadoAoProjetoPorNome(Guid usuarioId, string nomeProjeto)
        {
            return _context.Projeto.Any(p => p.Autor == usuarioId && p.NomeProjeto == nomeProjeto);
        }
        public bool UsuarioEstaAssociadoAoProjeto(Guid usuarioId, Guid projetoId)
        {
            return _context.Projeto.Any(p => p.Autor == usuarioId && p.ProjetoId == projetoId);
        }
        public bool UsuarioEstaAssociadoAoProjetoPorId(Guid usuarioId, Guid projetoId)
        {
            return _context.Projeto.Any(p => p.Autor == usuarioId && p.ProjetoId == projetoId);
        }

        // ================================= FUNCOES ==========================================
        public string? ObterLinkRepositorioPorId(Guid id)
        {
            var projeto = _context.Projeto.Find(id);
            return projeto?.LinkRepositorio;
        }
        public void AssociarUsuarioAoProjeto(Guid usuarioId, Guid projetoId)
        {
            var usuario = _context.Usuario.Find(usuarioId);
            var projeto = _context.Projeto.Find(projetoId);

            projeto.Usuario.Add(usuario);
            _context.SaveChanges();
        }
        public void DesassociarUsuarioDoProjeto(Guid usuarioId, Guid projetoId)
        {
            var usuario = _context.Usuario.Find(usuarioId);
            var projeto = _context.Projeto.Find(projetoId);
            projeto.Usuario.Remove(usuario);
            _context.SaveChanges();
        }
        public bool ProjetoPossuiColaboradores(Guid projetoId)
        {
            return _context.Projeto
                .Include(p => p.Usuario)
                .Any(p => p.ProjetoId == projetoId && p.Usuario.Any());
        }
    }
}
