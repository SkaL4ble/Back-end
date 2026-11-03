using API.Contexts;
using API.Domains;
using API.Interfaces;
namespace API.Applications.Services
{
    public class TipoMetricaService
    {
        private readonly ITipoMetricaRepository _tipoMetricaRepository;
        public TipoMetricaService(ITipoMetricaRepository tipoMetricaRepository)
        {
            _tipoMetricaRepository = tipoMetricaRepository;
        }
        public List<TipoMetrica> Listar()
        {
            return _tipoMetricaRepository.Listar();
        }
        public TipoMetrica BuscarPorId(int id)
        {
            return _tipoMetricaRepository.BuscarPorId(id);
        }
        public void Cadastrar(TipoMetrica tipoMetrica)
        {
            _tipoMetricaRepository.Cadastrar(tipoMetrica);
        }
        public void Atualizar(int id, TipoMetrica tipoMetrica)
        {
            _tipoMetricaRepository.Atualizar(id, tipoMetrica);
        }
        public void Remover(int id)
        {
            _tipoMetricaRepository.Remover(id);
        }
    }
}