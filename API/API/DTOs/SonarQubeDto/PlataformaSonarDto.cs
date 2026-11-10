namespace API.DTOs.SonarQubeDto
{
    public class PlataformaSonarDto
    {
        public string Tipo { get; set; } = null!;

        public string Chave { get; set; } = null!;

        public string? Url { get; set; }

        public bool Configurada { get; set; }
    }
}
