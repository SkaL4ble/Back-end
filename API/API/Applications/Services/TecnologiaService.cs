using API.Applications.Conversoes.ConversoesDeDto;
using API.Domains;
using API.DTOs.TecnologiaDto;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services;

public class TecnologiaService
{
    private const int TamanhoMaximoNome = 50;
    private readonly ITecnologiaRepository _repository;

    public TecnologiaService(ITecnologiaRepository repository) => _repository = repository;

    private Tecnologia BuscarOuFalhar(int id) =>
        _repository.ObterPorId(id)
        ?? throw new DomainException("Tecnologia não encontrada");

    private static string NormalizarNome(string? nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome da tecnologia é obrigatório");

        var normalizado = nome.Trim();
        if (normalizado.Length > TamanhoMaximoNome)
            throw new DomainException($"O nome da tecnologia deve ter no máximo {TamanhoMaximoNome} caracteres");

        return normalizado;
    }

    public List<ListarTecnologiaDto> ListarTecnologias() =>
        _repository.ListarTecnologias()
            .Select(ConversaoTecnologia.ConverterParaDto)
            .ToList();

    public ListarTecnologiaDto ObterPorId(int id) =>
        ConversaoTecnologia.ConverterParaDto(BuscarOuFalhar(id));

    public void AdicionarTecnologia(CriarTecnologiaDto dto)
    {
        var nome = NormalizarNome(dto.NomeTecnologia);
        if (_repository.NomeTecnologiaExiste(nome))
            throw new DomainException("Tecnologia já existe");

        _repository.Adicionar(new Tecnologia { NomeTecnologia = nome });
    }

    public void Atualizar(int id, CriarTecnologiaDto dto)
    {
        var atual = BuscarOuFalhar(id);
        var nome = NormalizarNome(dto.NomeTecnologia);
        var mudou = !string.Equals(atual.NomeTecnologia, nome, StringComparison.OrdinalIgnoreCase);

        if (mudou && _repository.NomeTecnologiaExiste(nome))
            throw new DomainException("Tecnologia já existe");

        _repository.Atualizar(id, new Tecnologia { TecnologiaId = id, NomeTecnologia = nome });
    }

    public void Deletar(int id)
    {
        BuscarOuFalhar(id);
        if (_repository.PossuiVinculos(id))
            throw new DomainException("Não é possível excluir: há usuários, métricas ou histórico vinculados a esta tecnologia");

        _repository.Deletar(id);
    }
}
