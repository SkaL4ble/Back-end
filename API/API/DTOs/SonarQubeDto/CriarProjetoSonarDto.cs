namespace API.DTOs.SonarQubeDto
{
    public class CriarProjetoSonarDto
    {
        public Guid ProjetoId { get; set; }

        public string Nome { get; set; } = null!;

        public string? Descricao { get; set; }

        public int WorkflowId { get; set; } = 1;
    }
}
