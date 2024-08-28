using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Shared.ConfigurationData;

namespace Shared.EmailSetting;
public class EmailSettings(IOptions<MailData> options) : IEmailSettings
{
    private readonly MailData _options = options.Value;

    public async Task SendEmailMessage(EmailResponse email)
    {
        var mail = new MimeMessage
        {
            Sender = MailboxAddress.Parse(_options.Email),
            Subject = email.Subject
        };

        mail.From.Add(new MailboxAddress("Admin", _options.Email));

        mail.To.Add(new MailboxAddress("User", email.To));

        var builder = new BodyBuilder
        {
            HtmlBody = email.Body
        };

        mail.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.SslOnConnect);

        await smtp.AuthenticateAsync(_options.Email, _options.Password);

        await smtp.SendAsync(mail);

        await smtp.DisconnectAsync(true);
    }
}