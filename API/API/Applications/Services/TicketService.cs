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

    // Retorna o ticket mesmo se estiver inativo (usado por operações que
    // precisam dar mensagem específica para ticket inativo).
    private Ticket BuscarOuFalhar(int id)
    {
        if (id <= 0)
            throw new DomainException("O ID do ticket deve ser maior que zero");

        return _repository.ObterPorId(id)
            ?? throw new NotFoundException("Ticket não encontrado");
    }

    // Soft delete: ticket inativo é tratado como inexistente em consultas normais.
    private Ticket BuscarAtivoOuFalhar(int id)
    {
        var ticket = BuscarOuFalhar(id);
        if (!ticket.Ativo)
            throw new NotFoundException("Ticket não encontrado");

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

    public List<ListarTicketDto> ListarTickets() => ParaDtos(_repository.ListarTickets());

    public List<ListarTicketDto> ListarTicketsPorUsuario(Guid usuarioId)
    {
        ValidarUsuario(usuarioId);
        return ParaDtos(_repository.ListarTicketsPorUsuario(usuarioId));
    }

    public List<ListarTicketDto> ListarTicketsPorProjeto(Guid projetoId)
    {
        ValidarProjeto(projetoId);
        return ParaDtos(_repository.ListarTicketsPorProjeto(projetoId));
    }

    public List<ListarTicketDto> ListarTicketsPorStatus(int statusTicketId)
    {
        ValidarStatusTicket(statusTicketId);
        return ParaDtos(_repository.ListarTicketsPorStatus(statusTicketId));
    }

    public List<ListarTicketDto> ListarTicketsPorTipo(int tipoTicketId)
    {
        ValidarTipoTicket(tipoTicketId);
        return ParaDtos(_repository.ListarTicketsPorTipo(tipoTicketId));
    }

    public List<ListarTicketDto> ListarTicketsPorUsuarioEProjeto(Guid usuarioId, Guid projetoId)
    {
        ValidarUsuario(usuarioId);
        ValidarProjeto(projetoId);
        return ParaDtos(_repository.ListarTicketsPorUsuarioEProjeto(usuarioId, projetoId));
    }

    public List<ListarTicketDto> ListarTicketsPorPrioridade(int prioridade)
    {
        ValidarPrioridade(prioridade);
        return ParaDtos(_repository.ListarTicketsPorPrioridade(prioridade));
    }

    public List<ListarTicketDto> ListarTicketsPorPeriodo(DateTime dataInicio, DateTime dataFim)
    {
        if (dataInicio > dataFim)
            throw new DomainException("A data de início não pode ser maior que a data de fim");

        return ParaDtos(_repository.ListarTicketsPorPeriodo(dataInicio, dataFim));
    }

    public List<ListarTicketDto> ListarTicketPorDataForaDoPrazo() =>
        ParaDtos(_repository.ListarTicketPorDataForaDoPrazo());

    public List<ListarTicketDto> ListarTicketPorDataForaDoPrazoPorUsuario(Guid usuarioId)
    {
        ValidarUsuario(usuarioId);
        return ParaDtos(_repository.ListarTicketPorDataForaDoPrazoPorUsuario(usuarioId));
    }

    public List<ListarTicketDto> ListarTicketPorDataForaDoPrazoPorProjeto(Guid projetoId)
    {
        ValidarProjeto(projetoId);
        return ParaDtos(_repository.ListarTicketPorDataForaDoPrazoPorProjeto(projetoId));
    }

    public List<ListarTicketDto> ListarTicketPorDataDentroDoPrazo() =>
        ParaDtos(_repository.ListarTicketPorDataDentroDoPrazo());

    public List<ListarTicketDto> ListarTicketPorDataDentroDoPrazoPorUsuario(Guid usuarioId)
    {
        ValidarUsuario(usuarioId);
        return ParaDtos(_repository.ListarTicketPorDataDentroDoPrazoPorUsuario(usuarioId));
    }

    public List<ListarTicketDto> ListarTicketPorDataDentroDoPrazoPorProjeto(Guid projetoId)
    {
        ValidarProjeto(projetoId);
        return ParaDtos(_repository.ListarTicketPorDataDentroDoPrazoPorProjeto(projetoId));
    }
}
