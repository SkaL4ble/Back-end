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

        public List<Projeto> ListarProjetosPorStatusETipo(int statusId, int tipoId)
        {
            return _context.Projeto
                .Where(p => p.StatusProjetoId == statusId && p.TipoProjetoId == tipoId)
                .ToList();
        }

        public List<Projeto> ListarProjetosPorStatus(int statusId)
        {
            return _context.Projeto
                .Where(p => p.StatusProjetoId == statusId)
                .ToList();
        }

        public List<Projeto> ListarProjetosPorTipo(int tipoId)
        {
            return _context.Projeto
                .Where(p => p.TipoProjetoId == tipoId)
                .ToList();
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
            Projeto? projeto = _context.Projeto.Find(id);

            if (projeto == null)
            {
                return;
            }
            _context.Projeto.Remove(projeto);
            _context.SaveChangesAsync();
        }

        public void AssociarUsuarioAoProjeto(Guid projetoId, Guid usuarioId)
        {
            var projeto = _context.Projeto.FirstOrDefault(p => p.ProjetoId == projetoId);
            var usuario = _context.Usuario.FirstOrDefault(u => u.UsuarioId == usuarioId && u.Ativo);

            if (projeto is null || usuario is null)
                return;

            projeto.Usuario.Add(usuario);
            _context.SaveChanges();
        }

        public void DesassociarUsuarioDoProjeto(Guid projetoId, Guid usuarioId)
        {
            var projeto = _context.Projeto.FirstOrDefault(p => p.ProjetoId == projetoId);
            var usuario = _context.Usuario.FirstOrDefault(u => u.UsuarioId == usuarioId && u.Ativo);
            if (projeto is null || usuario is null)
                return;
            projeto.Usuario.Remove(usuario);
            _context.SaveChanges();
        }

        public List<Projeto> ListarProjetosPorUsuario(Guid usuarioId)
        {
            var usuario = _context.Usuario
                .Include(u => u.Projeto)
                .FirstOrDefault(u => u.UsuarioId == usuarioId && u.Ativo);
            return usuario?.Projeto.ToList() ?? new List<Projeto>();
        }

        public bool UsuarioEstaAssociadoAoProjeto(Guid usuarioId, Guid projetoId)
        {
            var projeto = _context.Projeto
                .Include(p => p.Usuario)
                .FirstOrDefault(p => p.ProjetoId == projetoId);
            return projeto?.Usuario.Any(u => u.UsuarioId == usuarioId) ?? false;
        }

        public bool UsuarioEstaAssociadoAoProjetoPorNome(Guid usuarioId, string nomeProjeto)
        {
            var projeto = _context.Projeto
                .Include(p => p.Usuario)
                .FirstOrDefault(p => p.NomeProjeto == nomeProjeto);
            return projeto?.Usuario.Any(u => u.UsuarioId == usuarioId) ?? false;
        }
        public bool UsuarioEstaAssociadoAoProjetoPorId(Guid usuarioId, Guid projetoId)
        {
            var projeto = _context.Projeto
                .Include(p => p.Usuario)
                .FirstOrDefault(p => p.ProjetoId == projetoId);
            return projeto?.Usuario.Any(u => u.UsuarioId == usuarioId) ?? false;
        }
        public List<Projeto> ListarProjetosPorUsuarioEStatus(Guid usuarioId, int statusId)
        {
            var usuario = _context.Usuario
                .Include(u => u.Projeto)
                .FirstOrDefault(u => u.UsuarioId == usuarioId && u.Ativo);
            return usuario?.Projeto
                .Where(p => p.StatusProjetoId == statusId)
                .ToList() ?? new List<Projeto>();
        }
        public List<Projeto> ListarProjetosPorUsuarioETipo(Guid usuarioId, int tipoId)
        {
            var usuario = _context.Usuario
                .Include(u => u.Projeto)
                .FirstOrDefault(u => u.UsuarioId == usuarioId && u.Ativo);
            return usuario?.Projeto
                .Where(p => p.TipoProjetoId == tipoId)
                .ToList() ?? new List<Projeto>();
        }

        public List<Projeto> ListarProjetosPorUsuarioStatusETipo(Guid usuarioId, int statusId, int tipoId)
        {
            var usuario = _context.Usuario
                .Include(u => u.Projeto)
                .FirstOrDefault(u => u.UsuarioId == usuarioId && u.Ativo);
            return usuario?.Projeto
                .Where(p => p.StatusProjetoId == statusId && p.TipoProjetoId == tipoId)
                .ToList() ?? new List<Projeto>();
        }

        public void DeletarProjeto(Projeto? projeto)
        {
            if (projeto == null)
            {
                return;
            }
            _context.Projeto.Remove(projeto);
            _context.SaveChanges();
        }
    }
}
