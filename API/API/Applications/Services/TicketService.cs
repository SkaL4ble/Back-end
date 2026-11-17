using API.Applications.Conversoes;
using API.Domains;
using API.DTOs.TicketDto;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services;

public class TicketService
{
    private const int PrioridadeMinima = 1;
    private const int PrioridadeMaxima = 5;
    private const int TamanhoMinimoNome = 3;
    private const int TamanhoMaximoNome = 125;
    private readonly ITicketRepository _repository;

    public TicketService(ITicketRepository repository) => _repository = repository;

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

    private static string NormalizarNome(string? nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome do ticket é obrigatório");

        var normalizado = nome.Trim();
        if (normalizado.Length < TamanhoMinimoNome)
            throw new DomainException($"O nome do ticket não pode ter menos de {TamanhoMinimoNome} caracteres");
        if (normalizado.Length > TamanhoMaximoNome)
            throw new DomainException($"O nome do ticket não pode ter mais de {TamanhoMaximoNome} caracteres");

        return normalizado;
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

    private void ValidarUsuario(Guid usuarioId)
    {
        if (!_repository.UsuarioExiste(usuarioId))
            throw new DomainException("Usuário não encontrado ou inativo");
    }

    private void ValidarProjeto(Guid projetoId)
    {
        if (!_repository.ProjetoExiste(projetoId))
            throw new DomainException("Projeto não encontrado ou inativo");
    }

    private void ValidarTipoTicket(int tipoTicketId)
    {
        if (!_repository.TipoTicketExiste(tipoTicketId))
            throw new DomainException("Tipo de ticket não encontrado");
    }

    private void ValidarStatusTicket(int statusTicketId)
    {
        if (!_repository.StatusTicketExiste(statusTicketId))
            throw new DomainException("Status de ticket não encontrado");
    }

    private void ValidarParticipacaoNoProjeto(Guid usuarioId, Guid projetoId)
    {
        if (!_repository.UsuarioParticipaDoProjeto(usuarioId, projetoId))
            throw new DomainException("O usuário não participa do projeto do ticket");
    }

    private void ValidarRelacionamentos(Guid projetoId, int tipoTicketId,
        int statusTicketId, Guid? usuarioResponsavel)
    {
        ValidarProjeto(projetoId);
        ValidarTipoTicket(tipoTicketId);
        ValidarStatusTicket(statusTicketId);

        if (usuarioResponsavel.HasValue)
        {
            ValidarUsuario(usuarioResponsavel.Value);
            ValidarParticipacaoNoProjeto(usuarioResponsavel.Value, projetoId);
        }
    }

    private static List<ListarTicketDto> ParaDtos(IEnumerable<Ticket> tickets) =>
        tickets.Select(ConversaoTicket.ConverterParaDto).ToList();

    //========================================= CRUD =========================================
    public ListarTicketDto ObterPorId(int id) =>
        ConversaoTicket.ConverterParaDto(BuscarAtivoOuFalhar(id));

    public void AdicionarTicket(CriarTicketDto dto)
    {
        var nome = NormalizarNome(dto.NomeTicket);
        ValidarPrioridade(dto.Prioridade);
        ValidarRelacionamentos(dto.ProjetoId, dto.TipoTicketId,
            dto.StatusTicketId, dto.UsuarioResponsavel);

        var agora = DateTime.UtcNow;
        ValidarDatas(agora, dto.DataPrazo, null);

        _repository.AdicionarTicket(new Ticket
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
        });
    }

    public void Atualizar(int id, AtualizarTicketDto dto)
    {
        var ticket = BuscarAtivoOuFalhar(id);
        var nome = NormalizarNome(dto.NomeTicket);
        ValidarPrioridade(dto.Prioridade);
        ValidarRelacionamentos(dto.ProjetoId, dto.TipoTicketId,
            dto.StatusTicketId, dto.UsuarioResponsavel);
        ValidarDatas(ticket.DataCriacao, dto.DataPrazo, dto.DataEntrega);

        ConversaoTicket.AplicarAtualizacao(ticket, new AtualizarTicketDto
        {
            NomeTicket = nome,
            Prioridade = dto.Prioridade,
            DataPrazo = dto.DataPrazo,
            DataEntrega = dto.DataEntrega,
            UsuarioResponsavel = dto.UsuarioResponsavel,
            ProjetoId = dto.ProjetoId,
            TipoTicketId = dto.TipoTicketId,
            StatusTicketId = dto.StatusTicketId
        });

        _repository.AtualizarTicket(ticket);
    }

    public void Deletar(int id)
    {
        // Ticket já inativo (ou inexistente) => 404, o que torna o DELETE repetido previsível.
        var ticket = BuscarAtivoOuFalhar(id);
        _repository.DeletarTicket(ticket);
    }

    //==================================== FUNCOES ====================================
    public void MudarEstadoTicket(int ticketId, int statusTicketId)
    {
        var ticket = BuscarOuFalhar(ticketId);
        ValidarStatusTicket(statusTicketId);

        if (!ticket.Ativo)
            throw new DomainException("Não é possível alterar um ticket inativo");
        if (ticket.StatusTicketId == statusTicketId)
            throw new DomainException("O ticket já está com este status");

        _repository.MudarEstadoTicket(ticketId, statusTicketId);
    }

    public void TransferirResponsavel(int ticketId, Guid usuarioId)
    {
        var ticket = BuscarOuFalhar(ticketId);
        if (!ticket.Ativo)
            throw new DomainException("Não é possível transferir um ticket inativo");

        ValidarUsuario(usuarioId);
        if (ticket.UsuarioResponsavel == usuarioId)
            throw new DomainException("Este usuário já é o responsável pelo ticket");

        ValidarParticipacaoNoProjeto(usuarioId, ticket.ProjetoId);

        _repository.TransferirResponsavel(ticketId, usuarioId);
    }

    public void AdicionarColaborador(int ticketId, Guid usuarioId)
    {
        var ticket = BuscarOuFalhar(ticketId);
        if (!ticket.Ativo)
            throw new DomainException("Não é possível adicionar colaborador a um ticket inativo");

        ValidarUsuario(usuarioId);
        if (ticket.UsuarioResponsavel == usuarioId)
            throw new DomainException("O responsável não pode ser adicionado como colaborador");

        ValidarParticipacaoNoProjeto(usuarioId, ticket.ProjetoId);

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

        if (filtro.UsuarioId.HasValue)
        {
            ValidarUsuario(filtro.UsuarioId.Value);
        }

        if (filtro.ProjetoId.HasValue)
        {
            ValidarProjeto(filtro.ProjetoId.Value);
        }

        if (filtro.StatusTicketId.HasValue)
        {
            ValidarStatusTicket(filtro.StatusTicketId.Value);
        }

        if (filtro.TipoTicketId.HasValue)
        {
            ValidarTipoTicket(filtro.TipoTicketId.Value);
        }

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
        if(filtro.ProjetoId != Guid.Empty)
            ValidarProjeto(filtro.ProjetoId);
        else if(filtro.UsuarioId != Guid.Empty)
            ValidarUsuario(filtro.UsuarioId);

        return ParaDtos(_repository.ListarTicketsForaDoPrazo(filtro));
    }

    public List<ListarTicketDto> ListarTicketsDentroDoPrazo(FiltroTicketForaOuDentro filtro)
    {
        if (filtro.ProjetoId != Guid.Empty)
            ValidarProjeto(filtro.ProjetoId);
        else if (filtro.UsuarioId != Guid.Empty)
            ValidarUsuario(filtro.UsuarioId);

        return ParaDtos(_repository.ListarTicketsDentroDoPrazo(filtro));
    }
}
