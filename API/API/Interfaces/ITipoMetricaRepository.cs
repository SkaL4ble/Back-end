using API.Domains;

namespace API.Interfaces
{
    public interface ITipoMetricaRepository
    {
        List<TipoMetrica> ListarTiposMetrica();
        TipoMetrica? ObterPorId(int id);
        bool NomeTipoMetricaExiste(string nome);
        bool PossuiProjetos(int id);
        void Adicionar(TipoMetrica tipoMetrica);
        void Atualizar(int id, TipoMetrica tipoMetrica);
        void Deletar(int id);   
    }
}