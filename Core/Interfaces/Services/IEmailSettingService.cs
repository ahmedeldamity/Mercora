using Core.Dtos;

namespace Core.Interfaces.Services;
public interface IEmailSettingService
{
    public Task SendEmailMessage(EmailResponse email);
}