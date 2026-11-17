using API.Domains;
using API.DTOs.ProjetoDto;

namespace API.Interfaces;

public interface IProjetoRepository
{
    // ================================= CRUD =================================
    Projeto? ObterPorId(Guid id);
    void CadastrarProjeto(Projeto projeto);
    void AtualizarProjeto(Guid id, Projeto projeto);
    void DeletarProjeto(Guid id);

    // ================================ LISTAGEM =================================
    List<Projeto> ListarProjetos(FiltroProjetoDto filtro);
    List<Projeto> ListarProjetosForaDoPrazo(FiltroProjetoDentroOuForaPrazo filtro);
    List<Projeto> ListarProjetosDentroDoPrazo(FiltroProjetoDentroOuForaPrazo filtro);

    // ================================ VALIDAÇÕES =================================
    bool NomeProjetoExiste(string nome, Guid? idIgnorado = null);
    bool LinkRepositorioExiste(string linkRepositorio, Guid? idIgnorado = null);
    bool UsuarioExiste(Guid? usuarioId);
    bool StatusExiste(int? statusId);
    bool TipoExiste(int? tipoId);
    bool UsuarioEstaAssociadoAoProjetoPorNome(Guid usuarioId, string nomeProjeto);
    bool UsuarioEstaAssociadoAoProjeto(Guid usuarioId, Guid projetoId);
    bool UsuarioEstaAssociadoAoProjetoPorId(Guid usuarioId, Guid projetoId);
    bool ProjetoPossuiColaboradores(Guid projetoId);

    // ================================ FUNCOES =================================
    void AtualizarColaboradores(Guid projetoId, IEnumerable<Guid> usuariosIds);
    string? ObterLinkRepositorioPorId(Guid id);
    void AssociarUsuarioAoProjeto(Guid usuarioId, Guid projetoId);
    void DesassociarUsuarioDoProjeto(Guid usuarioId, Guid projetoId);
}
