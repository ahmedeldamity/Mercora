

namespace BlazorEcommerce.Server.Health;
public class MailHealthCheck(IOptions<MailData> mailData) : IHealthCheck
{
    private readonly MailData _mailData = mailData.Value;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(_mailData.Host, _mailData.Port, SecureSocketOptions.SslOnConnect, cancellationToken);

            await smtp.AuthenticateAsync(_mailData.Email, _mailData.Password, cancellationToken);

            return await Task.FromResult(HealthCheckResult.Healthy());
        }
        catch (Exception exception)
        {
            return await Task.FromResult(HealthCheckResult.Unhealthy(exception: exception));
        }
    }
}