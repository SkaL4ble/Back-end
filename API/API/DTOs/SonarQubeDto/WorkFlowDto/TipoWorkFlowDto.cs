namespace API.DTOs.SonarQubeDto.WorkFlowDto
{
    public class TipoWorkFlowDto
    {
        // Valor enviado de volta no endpoint de geração (ex.: "dotnet")
        public string Valor { get; set; } = null!;

        // Texto exibido no botão (ex.: ".NET")
        public string Nome { get; set; } = null!;
    }
}
