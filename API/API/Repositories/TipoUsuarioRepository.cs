using API.Contexts;
using API.Domains;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class TipoUsuarioRepository : ITipoUsuarioRepository
    {
        private readonly SkaL4bleContext _context;

        public TipoUsuarioRepository(SkaL4bleContext context)
        {
            _context = context;
        }

        public List<TipoUsuario> Listar()
        {
            return _context.TipoUsuario.AsNoTracking()
                .OrderBy(u => u.NomeTipoUsuario).ToList();
        }

        public TipoUsuario BuscarPorId(Guid id)
        {
            return _context.TipoUsuario.AsNoTracking()
                .FirstOrDefault(u => u.TipoUsuarioId == id);
        }

        public TipoUsuario BuscarPorCodigo(string codigo)
        {
            return _context.TipoUsuario
                .AsNoTracking()
                .FirstOrDefault(t => t.CodigoTipoUsuario == codigo);
        }

        public bool TipoUsuarioExistente(Guid id)
        {
            return _context.TipoUsuario.AsNoTracking().Any(u => u.TipoUsuarioId == id);
        }

        public bool TipoUsuarioComMesmoNomeExistente(string nome, Guid id)
        {
            return _context.TipoUsuario
                .AsNoTracking()
                .Any(u => u.NomeTipoUsuario.ToLower() == nome.ToLower() && u.TipoUsuarioId != id);
        }

        public void Cadastrar(TipoUsuario tipoUsuario)
        {
            _context.TipoUsuario.Add(tipoUsuario);
            _context.SaveChanges();
        }

        public void Atualizar(TipoUsuario tipoUsuario, Guid id)
        {
            if (tipoUsuario == null)
            {
                return;
            }

            TipoUsuario tipoUsuarioBanco = _context.TipoUsuario.Find(tipoUsuario.TipoUsuarioId);

            if (tipoUsuarioBanco == null)
            {
                return;
            }

            tipoUsuarioBanco.NomeTipoUsuario = tipoUsuario.NomeTipoUsuario;
            _context.SaveChanges();
        }
    }
}