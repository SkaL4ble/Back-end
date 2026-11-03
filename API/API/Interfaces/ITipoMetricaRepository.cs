using API.Domains;

namespace API.Interfaces
{
    public interface ITipoMetricaRepository
    {
        List<TipoMetrica> Listar();
        TipoMetrica BuscarPorId(int id);
        void Cadastrar(TipoMetrica tipoMetrica);
        void Atualizar(int id, TipoMetrica tipoMetrica);
        void Remover(int id);
    }
}