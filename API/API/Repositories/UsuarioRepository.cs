using API.Contexts;
using API.Domains;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class UsuarioRepository
    {
        private readonly SkaL4bleContext _context;

        public UsuarioRepository(SkaL4bleContext context)
        {
            _context = context;
        }

        public List<Usuario> Listar()
        {
            return _context.Usuario.AsNoTracking()
                .OrderBy(u => u.NomeUsuario).ToList();
        }

        public Usuario BuscarPorId(Guid id)
        {
            return _context.Usuario.AsNoTracking()
                .FirstOrDefault(u => u.UsuarioId == id);
        }

        public bool UsuarioExistente(Guid id)
        {
            return _context.Usuario.AsNoTracking().Any(u => u.UsuarioId == id);
        }

        public bool EmailExistente(string email, Guid id)
        {
            return _context.Usuario
                .AsNoTracking()
                .Any(u => u.EmailPessoal.ToLower() == email.ToLower() && u.UsuarioId != id);
        }

        public bool EmailGitExistente(string email, Guid id)
        {
            return _context.UsuarioEmailGit
                .AsNoTracking()
                .Any(u => u.EmailGit.ToLower() == email.ToLower() && u.UsuarioId != id);
        }

        public void Cadastrar(Usuario usuario)
        {
            _context.Usuario.Add(usuario);
            _context.SaveChanges();
        }

        public void Atualizar(Usuario usuario, Guid id)
        {
            if (usuario == null)
            {
                return;
            }
            Usuario usuarioBanco = _context.Usuario.Find(usuario.UsuarioId);
            if (usuarioBanco == null)
            {
                return;
            }
            _context.Entry(usuarioBanco).CurrentValues.SetValues(usuario);
            _context.SaveChanges();
        }

        public void Inativar(Guid id)
        {
            Usuario usuario = _context.Usuario.Find(id);
            if (usuario == null)
            {
                return;
            }
            usuario.Ativo = false;
            _context.SaveChanges();
        }
    }
}