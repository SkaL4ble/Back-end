namespace API.DTOs.SonarQubeDto.WorkFlowDto
{
    public class SecretWorkFlowDto
    {
        public string Nome { get; set; } = null!;

        public string Descricao { get; set; } = null!;

        // Preenchido só quando o valor não é sensível (ex.: a URL do SonarQube)
        public string? Valor { get; set; }
    }
}
