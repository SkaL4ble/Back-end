namespace API.DTOs;

public class DemandaTecnologiaDto
{
    public int TecnologiaId { get; set; }

    public string NomeTecnologia { get; set; }
        = string.Empty;

    public int QuantidadeTickets { get; set; }

    public int QuantidadeProjetos { get; set; }
}