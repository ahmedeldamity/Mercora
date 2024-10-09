using BlazorEcommerce.Application.Dtos;

namespace BlazorEcommerce.Application.Interfaces.Services;
public interface IEmailSettingService
{
    public Task SendEmailMessage(EmailResponse email);
}