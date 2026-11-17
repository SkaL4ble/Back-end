using API.Domains;
using API.DTOs.ProjetoDto;

namespace API.Interfaces
{
    public interface IProjetoRepository
    {
         //================================= CRUD ==========================================
         Projeto? ObterPorId(Guid id);
        public void CadastrarProjeto(Projeto projeto);
        public void AtualizarProjeto(Guid id, Projeto projeto);
        public void DeletarProjeto(Guid id);

        //================================= LISTAGENS ==========================================
        public List<Projeto> ListarProjetos(FiltroProjetoDto filtro);

        // ================================= VALIDAÇÕES ==========================================
        public bool NomeProjetoExiste(string nome);
        public bool LinkRepositorioExiste(string linkRepositorio);
        public bool UsuarioExiste(Guid? usuarioId);
        public bool StatusExiste(int? statusId);
        public bool TipoExiste(int? tipoId);
        public bool UsuarioEstaAssociadoAoProjetoPorNome(Guid usuarioId, string nomeProjeto);
        public bool UsuarioEstaAssociadoAoProjeto(Guid usuarioId, Guid projetoId);
        public bool UsuarioEstaAssociadoAoProjetoPorId(Guid usuarioId, Guid projetoId);

        // ================================= FUNCOES ==========================================
        public string? ObterLinkRepositorioPorId(Guid id);
        public void AssociarUsuarioAoProjeto(Guid usuarioId, Guid projetoId);
        public void DesassociarUsuarioDoProjeto(Guid usuarioId, Guid projetoId);
        bool ProjetoPossuiColaboradores(Guid projetoId);
    }
}
