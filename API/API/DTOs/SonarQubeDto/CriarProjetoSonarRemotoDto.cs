namespace API.DTOs.SonarQubeDto
{
    public class CriarProjetoSonarRemotoDto
    {
        public Guid ProjetoId { get; set; }

        // Valores retornados por GET api/SonarQube/plataformas
        public string Plataforma { get; set; } = null!;
        public string Chave { get; set; } = null!;

        public string Organizacao { get; set; } = null!;

        // Formato "organizacao/repositorio"
        public string Repositorio { get; set; } = null!;

        public string TokenGithub { get; set; } = null!;

        public string? Descricao { get; set; }
    }
}
