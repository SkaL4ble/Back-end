using API.Applications.Conversoes.ConversoesDeDto;
using API.Domains;
using API.DTOs.TipoTicketDto;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services;

public class TipoTicketService
{
    private const int TamanhoMaximoNome = 30;
    private const int TamanhoMinimoNome = 3;
    private readonly ITipoTicketRepository _repository;

    public TipoTicketService(ITipoTicketRepository repository) => _repository = repository;

    private TipoTicket BuscarOuFalhar(int id) =>
        _repository.ObterPorId(id)
        ?? throw new DomainException("Tipo de ticket não encontrado");

    private static string NormalizarNome(string? nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome do tipo de ticket é obrigatório");

        var normalizado = nome.Trim();
        if (normalizado.Length > TamanhoMaximoNome)
            throw new DomainException($"O nome do tipo de ticket deve ter no máximo {TamanhoMaximoNome} caracteres");
        else if(normalizado.Length < TamanhoMinimoNome)
            throw new DomainException($"O nome do tipo de ticket deve ter no mínimo {TamanhoMinimoNome} caracteres");

        return normalizado;
    }

    public List<ListarTipoTicketDto> ListarTiposTicket() =>
        _repository.ListarTiposTicket()
            .Select(ConversaoTipoTicket.ConverterParaDto)
            .ToList();

    public ListarTipoTicketDto ObterPorId(int id) =>
        ConversaoTipoTicket.ConverterParaDto(BuscarOuFalhar(id));

    public void AdicionarTipoTicket(CriarTipoTicketDto dto)
    {
        var nome = NormalizarNome(dto.NomeTipoTicket);
        if (_repository.NomeTipoTicketExiste(nome))
            throw new DomainException("Tipo de ticket já existe");

        _repository.Adicionar(new TipoTicket { NomeTipoTicket = nome });
    }

    public void Atualizar(int id, CriarTipoTicketDto dto)
    {
        var atual = BuscarOuFalhar(id);
        var nome = NormalizarNome(dto.NomeTipoTicket);
        var mudou = !string.Equals(atual.NomeTipoTicket, nome, StringComparison.OrdinalIgnoreCase);

        if (mudou && _repository.NomeTipoTicketExiste(nome))
            throw new DomainException("Tipo de ticket já existe");

        _repository.Atualizar(id, new TipoTicket { TipoTicketId = id, NomeTipoTicket = nome });
    }

    public void Deletar(int id)
    {
        BuscarOuFalhar(id);
        if (_repository.PossuiTickets(id))
            throw new DomainException("Não é possível excluir: há tickets vinculados a este tipo de ticket");

        _repository.Deletar(id);
    }
}
