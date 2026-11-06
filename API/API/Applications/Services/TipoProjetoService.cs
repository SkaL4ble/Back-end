using API.Applications.Conversoes;
using API.Domains;
using API.DTOs.TipoProjetoDto;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services;

public class TipoProjetoService
{
    private const int TamanhoMaximoNome = 30;
    private readonly ITipoProjetoRepository _repository;

    public TipoProjetoService(ITipoProjetoRepository repository) => _repository = repository;

    private TipoProjeto BuscarOuFalhar(int id) =>
        _repository.ObterPorId(id)
        ?? throw new DomainException("Tipo de projeto não encontrado");

    private static string NormalizarNome(string? nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome do tipo de projeto é obrigatório");

        var normalizado = nome.Trim();
        if (normalizado.Length > TamanhoMaximoNome)
            throw new DomainException($"O nome do tipo de projeto deve ter no máximo {TamanhoMaximoNome} caracteres");

        return normalizado;
    }

    public List<ListarTipoProjetoDto> ListarTiposProjeto() =>
        _repository.ListarTiposProjeto()
            .Select(ConversaoTipoProjeto.ConverterParaDto)
            .ToList();

    public ListarTipoProjetoDto ObterPorId(int id) =>
        ConversaoTipoProjeto.ConverterParaDto(BuscarOuFalhar(id));

    public void AdicionarTipoProjeto(CriarTipoProjetoDto dto)
    {
        var nome = NormalizarNome(dto.NomeTipoProjeto);
        if (_repository.NomeTipoProjetoExiste(nome))
            throw new DomainException("Tipo de projeto já existe");

        _repository.Adicionar(new TipoProjeto { NomeTipoProjeto = nome });
    }

    public void Atualizar(int id, CriarTipoProjetoDto dto)
    {
        var atual = BuscarOuFalhar(id);
        var nome = NormalizarNome(dto.NomeTipoProjeto);
        var mudou = !string.Equals(atual.NomeTipoProjeto, nome, StringComparison.OrdinalIgnoreCase);

        if (mudou && _repository.NomeTipoProjetoExiste(nome))
            throw new DomainException("Tipo de projeto já existe");

        _repository.Atualizar(id, new TipoProjeto { TipoProjetoId = id, NomeTipoProjeto = nome });
    }

    public void Deletar(int id)
    {
        BuscarOuFalhar(id);
        if (_repository.PossuiProjetos(id))
            throw new DomainException("Não é possível excluir: há projetos vinculados a este tipo de projeto");

        _repository.Deletar(id);
    }
}
