namespace API.Interfaces
{
    public interface IEmailService
    {
        Task EnviarEmailRedefinicaoSenhaAsync(
            string Email,
            string Token
        );
    }
}
