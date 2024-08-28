namespace Service.EmailSetting;
public interface IEmailSettings
{
    public Task SendEmailMessage(EmailResponse email);
}