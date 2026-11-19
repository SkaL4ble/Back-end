using System.Text.Json.Serialization;

namespace API.DTOs.SonarQubeDto
{
    public class ListarProjetoSonarDto
    {
        public Guid ProjetoSonarId { get; set; }

        public Guid ProjetoId { get; set; }
        public int WorkflowId { get; set; } = 1;
        public string ChaveSonar { get; set; } = null!;

        public string NomeSonar { get; set; } = null!;

        public string? Descricao { get; set; }

        public string Origem { get; set; } = null!;

        public string? Plataforma { get; set; }

        public string UrlProjeto { get; set; } = null!;

        public DateTime DataCriacao { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TokenSonarDto? TokenAnalise { get; set; }
    }
}
