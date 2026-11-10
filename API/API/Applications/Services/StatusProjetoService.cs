using API.Applications.Conversoes.ConversoesDeDto;
using API.Domains;
using API.DTOs.StatusProjetoDto;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services;

public class StatusProjetoService
{
    private const int TamanhoMaximoNome = 30;
    private readonly IStatusProjetoRepository _repository;

    public StatusProjetoService(IStatusProjetoRepository repository) => _repository = repository;

    private StatusProjeto BuscarOuFalhar(int id) =>
        _repository.ObterPorId(id)
        ?? throw new DomainException("Status de projeto não encontrado");

    private static string NormalizarNome(string? nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome do status de projeto é obrigatório");

        var normalizado = nome.Trim();
        if (normalizado.Length > TamanhoMaximoNome)
            throw new DomainException($"O nome do status de projeto deve ter no máximo {TamanhoMaximoNome} caracteres");

        return normalizado;
    }

    public List<ListarStatusProjetoDto> ListarStatusProjeto() =>
        _repository.ListarStatusProjeto()
            .Select(ConversaoStatusProjeto.ConverterParaDto)
            .ToList();

    public ListarStatusProjetoDto ObterPorId(int id) =>
        ConversaoStatusProjeto.ConverterParaDto(BuscarOuFalhar(id));

    public void AdicionarStatusProjeto(CriarStatusProjetoDto dto)
    {
        var nome = NormalizarNome(dto.NomeStatusProjeto);
        if (_repository.NomeStatusProjetoExiste(nome))
            throw new DomainException("Status de projeto já existe");

        _repository.Adicionar(new StatusProjeto { NomeStatusProjeto = nome });
    }

    public void Atualizar(int id, CriarStatusProjetoDto dto)
    {
        var atual = BuscarOuFalhar(id);
        var nome = NormalizarNome(dto.NomeStatusProjeto);
        var mudou = !string.Equals(atual.NomeStatusProjeto, nome, StringComparison.OrdinalIgnoreCase);

        if (mudou && _repository.NomeStatusProjetoExiste(nome))
            throw new DomainException("Status de projeto já existe");

        _repository.Atualizar(id, new StatusProjeto { StatusProjetoId = id, NomeStatusProjeto = nome });
    }

    public void Deletar(int id)
    {
        BuscarOuFalhar(id);
        if (_repository.PossuiProjetos(id))
            throw new DomainException("Não é possível excluir: há projetos vinculados a este status de projeto");

        _repository.Deletar(id);
    }
}
