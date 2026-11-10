namespace API.DTOs.SonarQubeDto
{
    public class CriarProjetoSonarLocalDto
    {
        public Guid ProjetoId { get; set; }

        public string Nome { get; set; } = null!;

        public string? Descricao { get; set; }

        public string TokenGithub { get; set; } = null!;
    }
}
