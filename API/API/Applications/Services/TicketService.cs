using API.Applications.Conversoes.ConversoesDeDto;
<<<<<<< HEAD
using API.Applications.Validacoes;
=======
using API.Applications.Services.Validacoes;
>>>>>>> 0fb51031abafc356f7cdc6125a16b23b059a7752
using API.Domains;
using API.DTOs.TicketDto;
using API.Exceptions;
using API.Interfaces;
using System.Net.NetworkInformation;

namespace API.Applications.Services;

public class TicketService
{
    private const int PrioridadeMinima = 1;
    private const int PrioridadeMaxima = 5;

    private readonly ITicketRepository _repository;
    private readonly MetricaService _metricaService;
    private readonly ValidacoesGerais _validacoes;

    public TicketService(ITicketRepository repository, MetricaService metricaService, ValidacoesGerais validacoes) 
    {
        _repository = repository;
        _metricaService = metricaService;
        _validacoes = validacoes;
    }

    //================================= VALIDAÇÕES ==========================================
    private Ticket BuscarOuFalhar(int id)
    {
        if (id <= 0)
            throw new DomainException("O ID do ticket deve ser maior que zero");

        return _repository.ObterPorId(id)
            ?? throw new DomainException("Ticket não encontrado");
    }
    private Ticket BuscarAtivoOuFalhar(int id)
    {
        var ticket = BuscarOuFalhar(id);
        if (!ticket.Ativo)
            throw new DomainException   ("Ticket não encontrado");

        return ticket;
    }

    private static void ValidarPrioridade(int prioridade)
    {
        if (prioridade < PrioridadeMinima || prioridade > PrioridadeMaxima)
            throw new DomainException($"A prioridade deve estar entre {PrioridadeMinima} e {PrioridadeMaxima}");
    }

    private static void ValidarDatas(DateTime dataCriacao, DateTime? prazo, DateTime? entrega)
    {
        if (prazo.HasValue && prazo.Value < dataCriacao)
            throw new DomainException("A data de prazo não pode ser anterior à data de criação");
        if (entrega.HasValue && entrega.Value < dataCriacao)
            throw new DomainException("A data de entrega não pode ser anterior à data de criação");
    }


    private void ValidarRelacionamentos(Guid projetoId, int tipoTicketId,
        int statusTicketId, Guid? usuarioResponsavel)
    {
        _validacoes.ValidarProjeto(projetoId);
        _validacoes.ValidarTipoTicket(tipoTicketId);
        _validacoes.ValidarStatusTicket(statusTicketId);

        if (usuarioResponsavel.HasValue)
        {
            _validacoes.ValidarUsuario(usuarioResponsavel);
<<<<<<< HEAD
            if(!_repository.UsuarioParticipaDoProjeto(usuarioResponsavel.Value, projetoId))
            {
                throw new DomainException("Usuario nao participa do projeto");
            }
=======
            ValidarParticipacaoNoProjeto(usuarioResponsavel.Value, projetoId);
>>>>>>> 0fb51031abafc356f7cdc6125a16b23b059a7752
        }
    }

    private static List<ListarTicketDto> ParaDtos(IEnumerable<Ticket> tickets) =>
        tickets.Select(ConversaoTicket.ConverterParaDto).ToList();

    //========================================= CRUD =========================================
    public ListarTicketDto ObterPorId(int id) =>
        ConversaoTicket.ConverterParaDto(BuscarAtivoOuFalhar(id));

    public async Task AdicionarTicket(CriarTicketDto dto)
    {
<<<<<<< HEAD
        var nome = _validacoes.NormalizarNome(dto.NomeTicket);
=======
        var nome = ValidacoesGerais.ValidarTamanho(ValidacoesGerais.NormalizarNome(dto.NomeTicket), 3, 125);
>>>>>>> 0fb51031abafc356f7cdc6125a16b23b059a7752
        ValidarPrioridade(dto.Prioridade);
        ValidarRelacionamentos(dto.ProjetoId, dto.TipoTicketId,
            dto.StatusTicketId, dto.UsuarioResponsavel);

        var agora = DateTime.UtcNow;
        ValidarDatas(agora, dto.DataPrazo, null);

        var ticket = new Ticket
        {
            NomeTicket = nome,
            Prioridade = dto.Prioridade,
            DataCriacao = agora,
            DataPrazo = dto.DataPrazo,
            DataEntrega = null,
            Ativo = true,
            UsuarioResponsavel = dto.UsuarioResponsavel,
            ProjetoId = dto.ProjetoId,
            TipoTicketId = dto.TipoTicketId,
            StatusTicketId = dto.StatusTicketId
        };
        _repository.AdicionarTicket(ticket);

        string status = _repository.PegarNomeStatusTicket(dto.StatusTicketId);

        if (status == "Concluido")
            await _metricaService.RegistrarTicketConcluidoAsync(ticket);
        else
        _metricaService.RegistrarTicketAtribuidoAsync(ticket);
    }

    public async Task Atualizar(int id, AtualizarTicketDto dto)
    {
        var ticket = BuscarAtivoOuFalhar(id);
<<<<<<< HEAD
        var nome = _validacoes.NormalizarNome(dto.NomeTicket);
=======
        var nome = ValidacoesGerais.ValidarTamanho(ValidacoesGerais.NormalizarNome(dto.NomeTicket), 3, 125);
>>>>>>> 0fb51031abafc356f7cdc6125a16b23b059a7752
        ValidarPrioridade(dto.Prioridade);
        ValidarRelacionamentos(dto.ProjetoId, dto.TipoTicketId,
            dto.StatusTicketId, dto.UsuarioResponsavel);
        ValidarDatas(ticket.DataCriacao, dto.DataPrazo, dto.DataEntrega);

        ConversaoTicket.AplicarAtualizacao(ticket, dto);

        string status = _repository.PegarNomeStatusTicket(id);

        if (status == "Concluido")
            await _metricaService.RegistrarTicketConcluidoAsync(ticket);

        _repository.AtualizarTicket(ticket);
    }

    public void Deletar(int id)
    {
        // Ticket já inativo (ou inexistente) => 404, o que torna o DELETE repetido previsível.
        var ticket = BuscarAtivoOuFalhar(id);
        _repository.DeletarTicket(ticket);
    }

    //==================================== FUNCOES ====================================
    public async Task MudarEstadoTicket(int ticketId, int statusTicketId)
    {
        var ticket = BuscarOuFalhar(ticketId);
        _validacoes.ValidarStatusTicket(statusTicketId);

        if (!ticket.Ativo)
            throw new DomainException("Não é possível alterar um ticket inativo");
        if (ticket.StatusTicketId == statusTicketId)
            throw new DomainException("O ticket já está com este status");

        string status = _repository.PegarNomeStatusTicket(ticketId);
        if (status == "Concluido")
            await _metricaService.RegistrarTicketConcluidoAsync(ticket);

        _repository.MudarEstadoTicket(ticketId, statusTicketId);
    }

    public void TransferirResponsavel(int ticketId, Guid usuarioId)
    {
        var ticket = BuscarOuFalhar(ticketId);
        if (!ticket.Ativo)
            throw new DomainException("Não é possível transferir um ticket inativo");

        _validacoes.ValidarUsuario(usuarioId);

        if (!_repository.UsuarioParticipaDoProjeto(usuarioId, ticket.ProjetoId))
        {
            throw new DomainException("Usuario nao participa do projeto");
        }

        _repository.TransferirResponsavel(ticketId, usuarioId);
    }

    public void AdicionarColaborador(int ticketId, Guid usuarioId)
    {
        var ticket = BuscarOuFalhar(ticketId);
        if (!ticket.Ativo)
            throw new DomainException("Não é possível adicionar colaborador a um ticket inativo");

        _validacoes.ValidarUsuario(usuarioId);

        if (!_repository.UsuarioParticipaDoTicket(usuarioId, ticketId))
        {
            throw new DomainException("Usuario ja esta no ticket");
        }

        if (!_repository.UsuarioParticipaDoProjeto(usuarioId, ticket.ProjetoId))
        {
            throw new DomainException("Usuario nao participa do projeto");
        }

        if (_repository.ColaboradorExiste(ticketId, usuarioId))
            throw new DomainException("Este usuário já é colaborador do ticket");

        _repository.AdicionarColaborador(ticketId, usuarioId);
    }

    public void RemoverColaborador(int ticketId, Guid usuarioId)
    {
        BuscarOuFalhar(ticketId);

        // Sem ValidarUsuario: um colaborador que foi desativado ainda precisa poder ser removido.
        if (!_repository.ColaboradorExiste(ticketId, usuarioId))
            throw new DomainException("Este usuário não é colaborador deste ticket");

        _repository.RemoverColaborador(ticketId, usuarioId);
    }

    //============================== LISTAGEM =================================
    public List<ListarTicketDto> ListarTickets(
    FiltroTicketDto filtro)
    {
        if (filtro.DataInicio.HasValue &&
            filtro.DataFim.HasValue &&
            filtro.DataInicio.Value > filtro.DataFim.Value)
        {
            throw new DomainException(
                "A data de início não pode ser maior que a data de fim");
        }

        _validacoes.ValidarProjeto(filtro.ProjetoId);
        _validacoes.ValidarUsuario(filtro.UsuarioId);
        _validacoes.ValidarTipoTicket(filtro.TipoTicketId);
        _validacoes.ValidarStatusTicket(filtro.StatusTicketId);

        if (filtro.Prioridade.HasValue)
        {
            ValidarPrioridade(filtro.Prioridade.Value);
        }

        var tickets = _repository.ListarTickets(filtro);

        return ParaDtos(tickets);
    }

    public List<ListarTicketDto> ListarTicketsPorPeriodo(DateTime dataInicio, DateTime dataFim)
    {
        if (dataInicio > dataFim)
            throw new DomainException("A data de início não pode ser maior que a data de fim");

        return ParaDtos(_repository.ListarTicketsPorPeriodo(dataInicio, dataFim));
    }

    public List<ListarTicketDto> ListarTicketsForaDoPrazo(FiltroTicketForaOuDentro filtro)
    {
        _validacoes.ValidarProjeto(filtro.ProjetoId);
        _validacoes.ValidarUsuario(filtro.UsuarioId);

        return ParaDtos(_repository.ListarTicketsForaDoPrazo(filtro));
    }

    public List<ListarTicketDto> ListarTicketsDentroDoPrazo(FiltroTicketForaOuDentro filtro)
    {
        _validacoes.ValidarProjeto(filtro.ProjetoId);
        _validacoes.ValidarUsuario(filtro.UsuarioId);

        return ParaDtos(_repository.ListarTicketsDentroDoPrazo(filtro));
    }
}
