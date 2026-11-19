namespace API.DTOs.SonarQubeDto.CommitDto
{
    public class PessoaCommitDto
    {
        public string? Nome { get; set; }

        public string? Email { get; set; }

        // Conta do GitHub. Fica nulo quando o e-mail do commit não está vinculado a nenhuma conta.
        public string? Login { get; set; }

        public DateTime? Data { get; set; }
    }
}
