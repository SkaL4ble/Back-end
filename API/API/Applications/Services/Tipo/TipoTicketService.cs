using API.Applications.Conversoes.ConversoesDeDto.ConversoesTipo;
using API.Applications.Services.Validacoes;
using API.Domains;
using API.DTOs.TipoDto.TipoTicketDto;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services.Tipo;

public class TipoTicketService
{
    private readonly ITipoTicketRepository _repository;
    private readonly ValidacoesGerais _validacoes;

    public TipoTicketService(ITipoTicketRepository repository, ValidacoesGerais validacoes) {
        _repository = repository;
        _validacoes = validacoes;
    } 

    private TipoTicket BuscarOuFalhar(int id) =>
        _repository.ObterPorId(id)
        ?? throw new DomainException("Tipo de ticket não encontrado");

    public List<ListarTipoTicketDto> ListarTiposTicket() =>
        _repository.ListarTiposTicket()
            .Select(ConversaoTipoTicket.ConverterParaDto)
            .ToList();

    public ListarTipoTicketDto ObterPorId(int id) =>
        ConversaoTipoTicket.ConverterParaDto(BuscarOuFalhar(id));

    public void AdicionarTipoTicket(CriarTipoTicketDto dto)
    {
        var nome = _validacoes.NormalizarNome(dto.NomeTipoTicket);
        if (_repository.NomeTipoTicketExiste(nome))
            throw new DomainException("Tipo de ticket já existe");

        _repository.Adicionar(new TipoTicket { NomeTipoTicket = nome });
    }

    public void Atualizar(int id, CriarTipoTicketDto dto)
    {
        var atual = BuscarOuFalhar(id);
        var nome = _validacoes.NormalizarNome(dto.NomeTipoTicket);
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
