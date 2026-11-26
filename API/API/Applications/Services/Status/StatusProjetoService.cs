using API.Applications.Conversoes.ConversoesDeDto.ConversoesStatus;
using API.Applications.Validacoes;
using API.Domains;
using API.DTOs.StatusDto.StatusProjetoDto;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services.Status;

public class StatusProjetoService
{
    private readonly IStatusProjetoRepository _repository;
    private readonly ValidacoesGerais _validacoes;

    public StatusProjetoService(IStatusProjetoRepository repository, ValidacoesGerais validacoes){ 
        _repository = repository;
        _validacoes = validacoes;
    }

    private StatusProjeto BuscarOuFalhar(int id) =>
        _repository.ObterPorId(id)
        ?? throw new DomainException("Status de projeto não encontrado");

    public List<ListarStatusProjetoDto> ListarStatusProjeto() =>
        _repository.ListarStatusProjeto()
            .Select(ConversaoStatusProjeto.ConverterParaDto)
            .ToList();

    public ListarStatusProjetoDto ObterPorId(int id) =>
        ConversaoStatusProjeto.ConverterParaDto(BuscarOuFalhar(id));

    public void AdicionarStatusProjeto(CriarStatusProjetoDto dto)
    {
        var nome = (dto.NomeStatusProjeto);
        if (_repository.NomeStatusProjetoExiste(nome))
            throw new DomainException("Status de projeto já existe");

        _repository.Adicionar(new StatusProjeto { NomeStatusProjeto = nome });
    }

    public void Atualizar(int id, CriarStatusProjetoDto dto)
    {
        var atual = BuscarOuFalhar(id);
        var nome = _validacoes.NormalizarNome(dto.NomeStatusProjeto);
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
