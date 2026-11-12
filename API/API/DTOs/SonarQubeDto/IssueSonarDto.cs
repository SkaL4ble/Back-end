namespace API.DTOs.SonarQubeDto
{
    public class IssueSonarDto
    {
        public string Chave { get; set; } = null!;
        public string? Autor { get; set; }
        public string? Responsavel { get; set; }

        public string Regra { get; set; } = null!;

        public string Severidade { get; set; } = null!;

        public string Tipo { get; set; } = null!;

        public string Status { get; set; } = null!;

        public string Mensagem { get; set; } = null!;

        public string Componente { get; set; } = null!;

        public int? Linha { get; set; }

        public DateTime? DataCriacao { get; set; }
    }
}
