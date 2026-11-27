using API.Applications.Conversoes.ConversoesDeDto;
using API.Applications.Validacoes;
using API.Domains;
using API.DTOs.TecnologiaDto;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services;

public class TecnologiaService
{
    private readonly ITecnologiaRepository _repository;
    private readonly ValidacoesGerais _validacao;

    public TecnologiaService(ITecnologiaRepository repository, ValidacoesGerais validacoes)
    {
        _repository = repository;
        _validacao = validacoes;
    }


    private Tecnologia BuscarOuFalhar(int id) =>
        _repository.ObterPorId(id)
        ?? throw new DomainException("Tecnologia não encontrada");


    public List<ListarTecnologiaDto> ListarTecnologias() =>
        _repository.ListarTecnologias()
            .Select(ConversaoTecnologia.ConverterParaDto)
            .ToList();

    public ListarTecnologiaDto ObterPorId(int id) =>
        ConversaoTecnologia.ConverterParaDto(BuscarOuFalhar(id));

    public void AdicionarTecnologia(CriarTecnologiaDto dto)
    {
        var nome = _validacao.NormalizarNome(dto.NomeTecnologia);
        if (_repository.NomeTecnologiaExiste(nome))
            throw new DomainException("Tecnologia já existe");

        _repository.Adicionar(new Tecnologia { NomeTecnologia = nome });
    }

    public void Atualizar(int id, CriarTecnologiaDto dto)
    {
        var atual = BuscarOuFalhar(id);
        var nome = _validacao.NormalizarNome(dto.NomeTecnologia);
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
