namespace API.DTOs.GameficacaoDto;

public class ListarGameficacaoDto
{
    public Guid UsuarioId { get; set; }

    public int ExperienciaTotal { get; set; }

    public int NivelGeral { get; set; }

    public DateTime DataAtualizacao { get; set; }

    public List<ListarTipoTicketGameficacaoDto>
        TiposTicket
    { get; set; } = new();

    public List<RecomendacaoTipoTicketDto>
        Recomendacoes
    { get; set; } = new();
}