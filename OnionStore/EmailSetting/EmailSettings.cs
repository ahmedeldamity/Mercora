using API.Settings;
using Core.Interfaces.EmailSetting;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace API.EmailSetting
{
    public class EmailSettings : IEmailSettings
    {
        private readonly MailSettings _options;
        private readonly ILogger<EmailSettings> _logger;

        public EmailSettings(IOptions<MailSettings> options, ILogger<EmailSettings> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task SendEmailMessage(Email email)
        {
            var mail = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_options.Email),
                Subject = email.Subject
            };

            mail.From.Add(new MailboxAddress("Admin", _options.Email));
            mail.To.Add(new MailboxAddress("User", email.To));

            var builder = new BodyBuilder();
            builder.HtmlBody = email.Body;
            mail.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            try
            {
                await smtp.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.SslOnConnect);
                await smtp.AuthenticateAsync(_options.Email, _options.Password);
                await smtp.SendAsync(mail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending email.");
                throw;
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}