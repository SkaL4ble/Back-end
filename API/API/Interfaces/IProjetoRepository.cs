using API.Domains;

namespace API.Interfaces
{
    public interface IProjetoRepository
    {
        List<Projeto> ListarProjetos();
        Projeto? ObterPorId(Guid id);
        Projeto? ObterPorNome(string nome);
        public bool NomeProjetoExiste(string nome);
        public bool LinkRepositorioExiste(string linkRepositorio);
        public bool UsuarioEstaAssociadoAoProjetoPorNome(Guid usuarioId, string nomeProjeto);
        public bool UsuarioEstaAssociadoAoProjeto(Guid usuarioId, Guid projetoId);
        public bool UsuarioEstaAssociadoAoProjetoPorId(Guid usuarioId, Guid projetoId);
        public string? ObterLinkRepositorioPorId(Guid id);
        public void CadastrarProjeto(Projeto projeto);
        public void AtualizarProjeto(Guid id, Projeto projeto);
        public void DeletarProjeto(Guid id);
        public void AssociarUsuarioAoProjeto(Guid usuarioId, Guid projetoId);
        public void DesassociarUsuarioDoProjeto(Guid usuarioId, Guid projetoId);
        public List<Projeto> ListarProjetosPorUsuario(Guid usuarioId);
        public List<Projeto> ListarProjetosPorStatus(int statusId);
        public List<Projeto> ListarProjetosPorTipo(int tipoId);
        public List<Projeto> ListarProjetosPorStatusETipo(int statusId, int tipoId);
    }
}
