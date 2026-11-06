using API.Applications.Conversoes;
using API.Domains;
using API.DTOs.StatusTicketDto;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services;

public class StatusTicketService
{
    private const int TamanhoMaximoNome = 30;
    private const int TamanhoMinimoNome = 3;
    private readonly IStatusTicketRepository _repository;

    public StatusTicketService(IStatusTicketRepository repository) => _repository = repository;

    private StatusTicket BuscarOuFalhar(int id) =>
        _repository.ObterPorId(id)
        ?? throw new DomainException("Status de ticket não encontrado");

    private static string NormalizarNome(string? nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome do status de ticket é obrigatório");

        var normalizado = nome.Trim();
        if (normalizado.Length > TamanhoMaximoNome)
            throw new DomainException($"O nome do status de ticket deve ter no máximo {TamanhoMaximoNome} caracteres");
        else if (normalizado.Length < TamanhoMinimoNome)
            throw new DomainException($"O nome do status de ticket deve ter no mínimo {TamanhoMinimoNome} caracteres");

        return normalizado;
    }

    public List<ListarStatusTicketDto> ListarStatusTicket() =>
        _repository.ListarStatusTicket()
            .Select(ConversaoStatusTicket.ConverterParaDto)
            .ToList();

    public ListarStatusTicketDto ObterPorId(int id) =>
        ConversaoStatusTicket.ConverterParaDto(BuscarOuFalhar(id));

    public void AdicionarStatusTicket(CriarStatusTicketDto dto)
    {
        var nome = NormalizarNome(dto.NomeStatusTicket);
        if (_repository.NomeStatusTicketExiste(nome))
            throw new DomainException("Status de ticket já existe");

        _repository.Adicionar(new StatusTicket { NomeStatusTicket = nome });
    }

    public void Atualizar(int id, CriarStatusTicketDto dto)
    {
        var atual = BuscarOuFalhar(id);
        var nome = NormalizarNome(dto.NomeStatusTicket);
        var mudou = !string.Equals(atual.NomeStatusTicket, nome, StringComparison.OrdinalIgnoreCase);

        if (mudou && _repository.NomeStatusTicketExiste(nome))
            throw new DomainException("Status de ticket já existe");

        _repository.Atualizar(id, new StatusTicket { StatusTicketId = id, NomeStatusTicket = nome });
    }

    public void Deletar(int id)
    {
        BuscarOuFalhar(id);
        if (_repository.PossuiTickets(id))
            throw new DomainException("Não é possível excluir: há tickets vinculados a este status de ticket");

        _repository.Deletar(id);
    }
}
