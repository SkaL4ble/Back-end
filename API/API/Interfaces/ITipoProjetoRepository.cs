using API.Domains;

namespace API.Interfaces
{
    public interface ITipoProjetoRepository
    {
        List<TipoProjeto> ListarTiposProjeto();
        TipoProjeto? ObterPorId(int id);
        bool NomeTipoProjetoExiste(string nome);
        bool PossuiProjetos(int id);
        void Adicionar(TipoProjeto tipoProjeto);
        void Atualizar(int id, TipoProjeto tipoProjeto);
        void Deletar(int id);
    }
}