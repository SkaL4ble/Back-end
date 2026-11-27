using API.Applications.Conversoes.ConversoesDeDto;
using API.Applications.Validacoes;
using API.Domains;
using API.DTOs.TicketDto;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services;

public class TicketService
{
    private const int PrioridadeMinima = 1;
    private const int PrioridadeMaxima = 3;

    private readonly ITicketRepository _repository;
    private readonly GameficacaoService _gameficacaoService;
    private readonly MetricaService _metricaService;
    private readonly ValidacoesGerais _validacoes;
    private readonly IStatusTicketRepository _statusTicketRepository;

    public TicketService(ITicketRepository repository, MetricaService metricaService, ValidacoesGerais validacoes, GameficacaoService gameficacaoService, IStatusTicketRepository statusTicketRepository)
    {
        _repository = repository;
        _metricaService = metricaService;
        _validacoes = validacoes;
        _gameficacaoService = gameficacaoService;
        _statusTicketRepository = statusTicketRepository;
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
            throw new DomainException("Ticket não encontrado");

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
            if (!_repository.UsuarioParticipaDoProjeto(usuarioResponsavel.Value, projetoId))
            {
                throw new DomainException("Usuario nao participa do projeto");
            }
        }
    }

    private static List<ListarTicketDto> ParaDtos(IEnumerable<Ticket> tickets) =>
        tickets.Select(ConversaoTicket.ConverterParaDto).ToList();

    //========================================= CRUD =========================================
    public ListarTicketDto ObterPorId(int id)
    {
        return ConversaoTicket.ConverterParaDto(BuscarAtivoOuFalhar(id));
    }

    public async Task AdicionarTicket(CriarTicketDto dto)
    {
        var nome = _validacoes.NormalizarNome(dto.NomeTicket);
        ValidarPrioridade(dto.Prioridade);
        ValidarRelacionamentos(dto.ProjetoId, dto.TipoTicketId,
            dto.StatusTicketId, dto.UsuarioResponsavel);
        var agora = DateTime.UtcNow;
        ValidarDatas(agora, dto.DataPrazo, null);
        bool concluido = _statusTicketRepository.StatusConcluido(dto.StatusTicketId);
        var ticket = new Ticket
        {
            NomeTicket = nome,
            Prioridade = dto.Prioridade,
            DataCriacao = agora,
            DataPrazo = dto.DataPrazo,
            DataEntrega = concluido ? agora : null,
            Ativo = true,
            UsuarioResponsavel = dto.UsuarioResponsavel,
            ProjetoId = dto.ProjetoId,
            TipoTicketId = dto.TipoTicketId,
            StatusTicketId = dto.StatusTicketId
        };
        _repository.AdicionarTicket(ticket);
        if (concluido)
            await _metricaService.RegistrarTicketConcluidoAsync(ticket);
        else
            await _metricaService.RegistrarTicketAtribuidoAsync(ticket);
    }
    public async Task Atualizar(int id, AtualizarTicketDto dto)
    {
        var ticket = BuscarAtivoOuFalhar(id);
        _validacoes.NormalizarNome(dto.NomeTicket);
        ValidarPrioridade(dto.Prioridade);
        ValidarRelacionamentos(dto.ProjetoId, dto.TipoTicketId,
            dto.StatusTicketId, dto.UsuarioResponsavel);
        ValidarDatas(ticket.DataCriacao, dto.DataPrazo, dto.DataEntrega);

        bool jaConcluido = _statusTicketRepository.StatusConcluido(ticket.StatusTicketId);
        ConversaoTicket.AplicarAtualizacao(ticket, dto);
        bool concluidoAgora = _statusTicketRepository.StatusConcluido(dto.StatusTicketId);
        if (concluidoAgora && !jaConcluido && !ticket.DataEntrega.HasValue)
            ticket.DataEntrega = DateTime.UtcNow;

        _repository.AtualizarTicket(ticket);
        if (concluidoAgora && !jaConcluido)
            await _metricaService.RegistrarTicketConcluidoAsync(ticket);
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

        bool jaConcluido = _statusTicketRepository.StatusConcluido(ticket.StatusTicketId);
        bool concluidoAgora = _statusTicketRepository.StatusConcluido(statusTicketId);
        ticket.StatusTicketId = statusTicketId;
        if (concluidoAgora && !jaConcluido && !ticket.DataEntrega.HasValue)
            ticket.DataEntrega = DateTime.UtcNow;

        _repository.MudarEstadoTicket(ticketId, statusTicketId);
        if (concluidoAgora && !jaConcluido)
            await _metricaService.RegistrarTicketConcluidoAsync(ticket);
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
