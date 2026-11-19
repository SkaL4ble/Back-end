using API.Domains;

namespace API.Interfaces
{
    public interface ITipoUsuarioRepository
    {
        List<TipoUsuario> Listar();

        TipoUsuario BuscarPorId(Guid id);

        bool TipoUsuarioExistente(Guid id);

        bool TipoUsuarioComMesmoNomeExistente(string nome, Guid id);

        TipoUsuario BuscarPorCodigo(string codigo);

        void Cadastrar(TipoUsuario tipoUsuario);

        void Atualizar(TipoUsuario tipoUsuario, Guid id);
    }
}