namespace API.DTOs.TicketDto;

public sealed class FiltroTicketDto
{
    public Guid? UsuarioId { get; set; }

    public Guid? ProjetoId { get; set; }

    public int? StatusTicketId { get; set; }

    public int? TipoTicketId { get; set; }

    public int? Prioridade { get; set; }

    public DateTime? DataInicio { get; set; }

    public DateTime? DataFim { get; set; }

    public string? Nome { get; set; }
}
