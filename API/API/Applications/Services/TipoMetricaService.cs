using API.Applications.Conversoes.ConversoesDeDto;
using API.Contexts;
using API.Domains;
using API.DTOs.StatusTicketDto;
using API.DTOs.TipoMetricaDto;
using API.Exceptions;
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
            return _tipoMetricaRepository.ListarTiposMetrica();
        }
        public TipoMetrica BuscarPorId(int id)
        {
            return _tipoMetricaRepository.ObterPorId(id);
        }
        public void AdicionarTipoMetrica(CriarTipoMetricaDto tipoMetricaDto)
        {
            tipoMetricaDto.NomeMetrica = (tipoMetricaDto.NomeMetrica);

            if (_tipoMetricaRepository.NomeTipoMetricaExiste(tipoMetricaDto.NomeMetrica))
                throw new DomainException("Tipo de métrica já existe");

            _tipoMetricaRepository.Adicionar(ConversaoTipoMetrica.ConverterParaDomain(tipoMetricaDto));
        }
        public void Atualizar(int id, TipoMetrica tipoMetrica)
        {
            _tipoMetricaRepository.Atualizar(id, tipoMetrica);
        }
        public void Remover(int id)
        {
            _tipoMetricaRepository.Deletar(id);
        }
    }
}