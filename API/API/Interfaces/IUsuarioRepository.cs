using API.Domains;
using API.DTOs.UsuarioDto;

namespace API.Interfaces
{
    public interface IUsuarioRepository
    {
        List<Usuario> Listar(FiltroUsuarioDto filtro);

        Usuario BuscarPorId(Guid id);


        void Cadastrar(Usuario usuario);

        void Atualizar(Usuario usuario, Guid id);

        Task<Usuario?> BuscarEmail(string email);

        Task<bool> AtualizarSenha(Guid id, byte[] senha);

        void Inativar(Guid id);

        //====================== VALIDACOES ===========================
        bool EmailExistente(string email, Guid id);

        bool EmailGitExistente(string emailGit, Guid id);
    }
}