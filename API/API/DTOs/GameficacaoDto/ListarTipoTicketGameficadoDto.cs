namespace API.DTOs.GameficacaoDto;

public class ListarTipoTicketGameficacaoDto
{
    public int TipoTicketId { get; set; }

    public string NomeTipoTicket { get; set; } = string.Empty;

    public decimal AfinidadeMedia { get; set; }

    public int ExperienciaTipoTicket { get; set; }

    public int NivelTipoTicket { get; set; }

    public int NumeroAtribuicoes { get; set; }
}