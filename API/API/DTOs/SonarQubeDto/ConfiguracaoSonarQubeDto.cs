using API.DTOs.SonarQubeDto.WorkFlowDto;

namespace API.DTOs.SonarQubeDto
{
    public class ConfiguracaoSonarQubeDto
    {
        public Guid ProjetoSonarId { get; set; }

        public string ChaveSonar { get; set; } = null!;

        public string TokenSonar { get; set; } = null!;

        public string UrlSonarQube { get; set; } = null!;

        public string Branch { get; set; } = null!;

        public string CaminhoArquivo { get; set; } = ".github/workflows/sonarqube.yml";

        public string ConteudoYaml { get; set; } = null!;

        public List<SecretWorkFlowDto> SecretsGithub { get; set; } = new();
    }
}
