namespace API.DTOs.SonarQubeDto
{
    public class ProjetoSonarDto
    {
        public Guid ProjetoSonarId { get; set; }

        public Guid ProjetoId { get; set; }

        public string ChaveSonar { get; set; } = null!;

        public string NomeSonar { get; set; } = null!;

        public string? Descricao { get; set; }

        public string Origem { get; set; } = null!;

        public string? Plataforma { get; set; }

        public string UrlProjeto { get; set; } = null!;

        public DateTime DataCriacao { get; set; }
    }
}
