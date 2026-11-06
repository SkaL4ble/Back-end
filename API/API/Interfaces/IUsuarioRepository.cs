using API.Domains;

namespace API.Interfaces
{
    public interface IUsuarioRepository
    {
        List<Usuario> Listar();

        Usuario BuscarPorId(Guid id);

        bool UsuarioExistente(Guid id);

        bool EmailExistente(string email, Guid id);

        bool EmailGitExistente(string emailGit, Guid id);

        void Cadastrar(Usuario usuario);

        void Atualizar(Usuario usuario, Guid id);

        void Inativar(Guid id);
    }
}