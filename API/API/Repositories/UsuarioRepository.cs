using API.Contexts;
using API.Domains;
using API.DTOs.UsuarioDto;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly SkaL4bleContext _context;

        public UsuarioRepository(SkaL4bleContext context)
        {
            _context = context;
        }

        private IQueryable<Usuario> UsuariosComRelacoes()
        {
            return _context.Usuario
                .Include(u => u.TipoUsuario)
                .Include(u => u.Cargo)
                .Include(u => u.Atuacao)
                .Include(u => u.UsuarioEmailGit)
                .Include(u => u.Projeto)
                .Include(u => u.Ticket)
                .Include(u => u.TecnologiaUsuario)
                .AsNoTracking();
        }

        public List<Usuario> Listar(FiltroUsuarioDto filtro)
        {
            var query = UsuariosComRelacoes();

            if (filtro.AtuacaoIds.Count > 0)
            {
                query = query.Where(u => filtro.AtuacaoIds.Contains(u.AtuacaoId));
            }

            if (filtro.CargoIds?.Count > 0)
            {
                query = query.Where(u => filtro.CargoIds.Contains(u.CargoId));
            }

            if (filtro.TipoUsuarioId.HasValue)
            {
                query = query.Where(u => u.TipoUsuarioId == filtro.TipoUsuarioId.Value);
            }

            if (filtro.ProjetosIds?.Count > 0)
            {
                query = query.Where(u => u.Projeto
                    .Any(p => filtro.ProjetosIds.Contains(p.ProjetoId)));
            }

            if (filtro.TecnologiasIds?.Count > 0)
            {
                query = query.Where(u => u.TecnologiaUsuario
                    .Any(t => filtro.TecnologiasIds.Contains(t.TecnologiaId)));
            }

            return query.ToList();
        }

        public Usuario BuscarPorId(Guid id)
        {
            return UsuariosComRelacoes()
                .AsNoTracking()
                .FirstOrDefault(u => u.UsuarioId == id);
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
            Usuario usuarioBanco = _context.Usuario.Find(id);
            if (usuarioBanco == null)
            {
                return;
            }
            _context.Entry(usuarioBanco).CurrentValues.SetValues(usuario);
            _context.SaveChanges();
        }

        public async Task<Usuario?> BuscarEmail(string email)
        {
            return await _context.Usuario
                .Include(u => u.TipoUsuario)
                .SingleOrDefaultAsync(u => u.EmailPessoal == email);
        }

        public async Task<bool> AtualizarSenha(Guid id, byte[] senha)
        {
            Usuario? usuario = await _context.Usuario.FindAsync(id);

            if (usuario is null)
            {
                return false;
            }

            usuario.Senha = senha;
            await _context.SaveChangesAsync();

            return true;
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

        //======================== VALIDACOES ===========================

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
    }
}