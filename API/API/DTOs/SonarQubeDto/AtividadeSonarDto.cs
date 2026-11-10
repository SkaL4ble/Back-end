namespace API.DTOs.SonarQubeDto
{
    public class AtividadeSonarDto
    {
        public string Chave { get; set; } = null!;

        public DateTime? Data { get; set; }

        public List<string> Eventos { get; set; } = new();
    }
}
