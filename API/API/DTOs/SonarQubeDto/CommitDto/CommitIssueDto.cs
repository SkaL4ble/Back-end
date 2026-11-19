namespace API.DTOs.SonarQubeDto.CommitDto
{
    public class CommitIssueDto
    {
        public string Revisao { get; set; } = null!;

        public string? Mensagem { get; set; }

        public string? UrlCommit { get; set; }

        public PessoaCommitDto Autor { get; set; } = new();

        public PessoaCommitDto Committer { get; set; } = new();
    }
}
