namespace API.DTOs.SonarQubeDto.ListagemDto;

public class IssueSonarDto
{
    public string Componente { get; set; } = string.Empty;
    public int? Linha { get; set; }
    public string? LinhaInicial { get; set; }
    public string? LinhaFinal { get; set; }

    public string Chave { get; set; } = string.Empty;
    public string Regra { get; set; } = string.Empty;
    public string Severidade { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;


    public string? ComoCorrigir { get; set; }
    public string? EsforcoRemediacao { get; set; }

    public string? Autor { get; set; }
    public DateTime? DataCriacao { get; set; }
}
