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
        bool UsuarioExistente(Guid id);

        bool EmailExistente(string email, Guid id);

        bool EmailGitExistente(string emailGit, Guid id);

        bool AtuacaoExiste(int id);
        bool CargoExiste(int id);
        bool TipoUsuarioExiste(Guid id);
        bool TecnologiaExiste(int id);
        bool ProjetosExiste(Guid id);
    }
}