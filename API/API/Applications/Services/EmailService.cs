using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

using API.Interfaces;
using MailKit;

namespace API.Applications.Services
{
    public class EmailService : IEmailService
    {
        public async Task EnviarEmailRedefinicaoSenhaAsync(
            string email,
            string token)
        {
            var host = Environment.GetEnvironmentVariable("EMAIL_HOST");
            var portString = Environment.GetEnvironmentVariable("EMAIL_PORT");
            var usuario = Environment.GetEnvironmentVariable("EMAIL_USER");
            var senha = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
            var remetente = Environment.GetEnvironmentVariable("EMAIL_FROM");

            if (string.IsNullOrWhiteSpace(host) ||
                string.IsNullOrWhiteSpace(portString) ||
                string.IsNullOrWhiteSpace(usuario) ||
                string.IsNullOrWhiteSpace(senha) ||
                string.IsNullOrWhiteSpace(remetente))
            {
                throw new InvalidOperationException(
                    "As configurações de e-mail não foram encontradas."
                );
            }

            if (!int.TryParse(portString, out int porta))
            {
                throw new InvalidOperationException(
                    "EMAIL_PORT inválido."
                );
            }

            var mensagem = new MimeMessage();

            // Remetente
            mensagem.From.Add(
                new MailboxAddress(
                    "SkaL4ble",
                    remetente
                )
            );

            // Destinatário
            mensagem.To.Add(
                MailboxAddress.Parse(email)
            );

            // Assunto
            mensagem.Subject = "Redefinição de senha - SkaL4ble";

            // Corpo do e-mail
            mensagem.Body = new TextPart("html")
            {
                Text = $@"
<!DOCTYPE html>
<html lang='pt-BR'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>

<body style='
    margin: 0;
    padding: 0;
    background-color: #FFFFFF;
    font-family: Arial, Helvetica, sans-serif;
'>

    <div style='
        max-width: 600px;
        margin: 40px auto;
        background-color: #FFFFFF;
        border-radius: 16px;
        overflow: hidden;
        box-shadow: 0 4px 20px rgba(0,0,0,0.08);
    '>

        <!-- Cabeçalho -->
        <div style='
            background-color: #5810B1;
            padding: 30px;
            text-align: center;
        '>
            <h1 style='
                margin: 0;
                color: #FFFFFF;
                font-size: 28px;
            '>
                SkaL4ble
            </h1>

            <p style='
                margin: 8px 0 0;
                color: #FFFFFF;
                font-size: 14px;
            '>
                Bem vindo(a), colaborador(a)!
            </p>
        </div>

        <!-- Conteúdo -->
        <div style='
            padding: 40px 35px;
            color: #333333;
        '>

            <h2 style='
                margin-top: 0;
                color: #222222;
                font-size: 24px;
            '>
                Redefinição de senha
            </h2>

            <p style='
                font-size: 16px;
                line-height: 1.6;
            '>
                Olá!
            </p>

            <p style='
                font-size: 16px;
                line-height: 1.6;
            '>
                Recebemos uma solicitação para redefinir a senha
                da sua conta no <strong>SkaL4ble</strong>.
            </p>

            <p style='
                font-size: 16px;
                line-height: 1.6;
            '>
                Utilize o código abaixo para continuar:
            </p>

            <!-- Código -->
            <div style='
                margin: 30px 0;
                padding: 25px;
                background-color: #FFFFFF;
                border: 1px solid #5810B1;
                border-radius: 12px;
                text-align: center;
            '>

                <p style='
                    margin: 0 0 10px;
                    color: #666666;
                    font-size: 13px;
                    text-transform: uppercase;
                    letter-spacing: 1px;
                '>
                    Código de redefinição
                </p>

                <div style='
                    color: #5810B1;
                    font-size: 25px;
                    font-weight: bold;
                    letter-spacing: 5px;
                '>
                    {token}
                </div>

            </div>

            <p style='
                font-size: 14px;
                line-height: 1.6;
                color: #666666;
            '>
                ⏱ Este código é válido por <strong>60 minutos</strong>.
            </p>

            <p style='
                font-size: 14px;
                line-height: 1.6;
                color: #666666;
            '>
                Se você não solicitou a redefinição da sua senha,
                pode ignorar este e-mail. Sua conta continuará segura.
            </p>

        </div>

        <!-- Rodapé -->
        <div style='
            background-color: #FFFFFF;
            padding: 25px;
            text-align: center;
            border-top: 1px solid #EEEEEE;
        '>

            <p style='
                margin: 0;
                color: #777777;
                font-size: 12px;
            '>
                © {DateTime.Now.Year} SkaL4ble
            </p>

            <p style='
                margin: 8px 0 0;
                color: #999999;
                font-size: 11px;
            '>
                Este é um e-mail automático. Por favor, não responda.
            </p>

        </div>

    </div>

</body>
</html>"
            };

            using var smtp = new SmtpClient();

            // Conecta ao servidor SMTP
            await smtp.ConnectAsync(
                host,
                porta,
                SecureSocketOptions.SslOnConnect
            );

            // Autentica
            await smtp.AuthenticateAsync(
                usuario,
                senha
            );

            // Envia
            await smtp.SendAsync(mensagem);

            // Desconecta
            await smtp.DisconnectAsync(true);
        }
    }
}
