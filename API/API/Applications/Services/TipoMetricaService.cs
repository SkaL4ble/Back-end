<<<<<<< HEAD
using API.Applications.Conversoes.ConversoesDeDto;
using API.Contexts;
using API.Domains;
using API.DTOs.StatusTicketDto;
using API.DTOs.TipoMetricaDto;
=======
using API.Applications.Services;
using API.Domains;
>>>>>>> fce64ec2d6d75cf75b8d1c0bcf73ce76e6e43367
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services;

public class TipoMetricaService
{
    private const int TamanhoMaximoNome = 50;
    private readonly ITipoMetricaRepository _repository;

    public TipoMetricaService(ITipoMetricaRepository repository) => _repository = repository;

    private TipoMetrica BuscarOuFalhar(int id) =>
        _repository.BuscarPorId(id)
        ?? throw new DomainException("Tipo de métrica não encontrado");

    private static string NormalizarNome(string? nome)
    {
<<<<<<< HEAD
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
=======
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome da métrica é obrigatório");

        var normalizado = nome.Trim();
        if (normalizado.Length > TamanhoMaximoNome)
            throw new DomainException($"O nome da métrica deve ter no máximo {TamanhoMaximoNome} caracteres");

        return normalizado;
>>>>>>> fce64ec2d6d75cf75b8d1c0bcf73ce76e6e43367
    }

    public List<TipoMetrica> Listar() =>
        _repository.Listar()
            .OrderBy(x => x.NomeMetrica)
            .ToList();

    public TipoMetrica BuscarPorId(int id) => BuscarOuFalhar(id);

    public void Cadastrar(TipoMetrica tipoMetrica)
    {
        var nome = NormalizarNome(tipoMetrica.NomeMetrica);
        // A interface atual não possui NomeMetricaExiste. Adicione essa função ao repository
        // antes de usar esta validação em produção.
        _repository.Cadastrar(new TipoMetrica { NomeMetrica = nome });
    }

    public void Atualizar(int id, TipoMetrica tipoMetrica)
    {
        BuscarOuFalhar(id);
        var nome = NormalizarNome(tipoMetrica.NomeMetrica);
        _repository.Atualizar(id, new TipoMetrica
        {
            TipoMetricaId = id,
            NomeMetrica = nome
        });
    }

    public void Remover(int id)
    {
        BuscarOuFalhar(id);
        // Adicione PossuiVinculos(int id) à ITipoMetricaRepository para bloquear
        // a remoção quando houver histórico ou consolidado associado.
        _repository.Remover(id);
    }
}
