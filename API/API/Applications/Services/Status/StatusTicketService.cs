using API.Applications.Conversoes.ConversoesDeDto.ConversoesStatus;
<<<<<<< HEAD
using API.Applications.Validacoes;
=======
using API.Applications.Services.Validacoes;
>>>>>>> 0fb51031abafc356f7cdc6125a16b23b059a7752
using API.Domains;
using API.DTOs.StatusDto.StatusTicketDto;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services.Status;

public class StatusTicketService
{
    private const int TamanhoMaximoNome = 30;
    private const int TamanhoMinimoNome = 3;
    private readonly IStatusTicketRepository _repository;
    private readonly ValidacoesGerais _validacoes;

    public StatusTicketService(IStatusTicketRepository repository, ValidacoesGerais validacoes) {
        _repository = repository;
        _validacoes = validacoes;
    } 

    private StatusTicket BuscarOuFalhar(int id) =>
        _repository.ObterPorId(id)
        ?? throw new DomainException("Status de ticket não encontrado");


    public List<ListarStatusTicketDto> ListarStatusTicket() =>
        _repository.ListarStatusTicket()
            .Select(ConversaoStatusTicket.ConverterParaDto)
            .ToList();

    public ListarStatusTicketDto ObterPorId(int id) =>
        ConversaoStatusTicket.ConverterParaDto(BuscarOuFalhar(id));

    public void AdicionarStatusTicket(CriarStatusTicketDto dto)
    {
        var nome = _validacoes.NormalizarNome(dto.NomeStatusTicket);
        if (_repository.NomeStatusTicketExiste(nome))
            throw new DomainException("Status de ticket já existe");

        _repository.Adicionar(new StatusTicket { NomeStatusTicket = nome });
    }

    public void Atualizar(int id, CriarStatusTicketDto dto)
    {
        var atual = BuscarOuFalhar(id);
        var nome = _validacoes.NormalizarNome(dto.NomeStatusTicket);
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
