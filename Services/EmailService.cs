using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Grandes_Amigos.Services
{
    public class EmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _smtpHost =
                configuration["Correo:SMTP_Host"]
                ?? throw new Exception("SMTP_Host no configurado.");
            _smtpPort = int.Parse(configuration["Correo:SMTP_Port"] ?? "587");
            _smtpUser =
                configuration["Correo:SMTP_User"]
                ?? throw new Exception("SMTP_User no configurado.");
            _smtpPass =
                configuration["Correo:SMTP_Pass"]
                ?? throw new Exception("SMTP_Pass no configurado.");
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string bodyHtml)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Grandes Amigos", _smtpUser));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = bodyHtml };

            try
            {
                using var client = new SmtpClient();
                await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpUser, _smtpPass);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error enviando correo: {ex}");
                throw;
            }
        }
    }
}
