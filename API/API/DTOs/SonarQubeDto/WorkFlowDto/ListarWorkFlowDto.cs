namespace API.DTOs.SonarQubeDto.WorkFlowDto
{
    public class ListarWorkFlowDto
    {
        public string Tipo { get; set; } = null!;

        // Onde o usuário deve salvar o arquivo no repositório
        public string CaminhoArquivo { get; set; } = null!;

        // Conteúdo do .yml, já com a chave do projeto
        public string Conteudo { get; set; } = null!;

        public List<string> Passos { get; set; } = new();

        public List<SecretWorkFlowDto> Secrets { get; set; } = new();

        public List<string> Avisos { get; set; } = new();
    }
}
