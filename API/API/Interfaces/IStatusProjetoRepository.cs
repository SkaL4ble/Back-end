using API.Domains;

namespace API.Interfaces
{
    public interface IStatusProjetoRepository
    {
        List<StatusProjeto> ListarStatusProjeto();
        StatusProjeto? ObterPorId(int id);
        bool NomeStatusProjetoExiste(string nome);
        bool PossuiProjetos(int id);
        void Adicionar(StatusProjeto statusProjeto);
        void Atualizar(int id, StatusProjeto statusProjeto);
        void Deletar(int id);
    }
}