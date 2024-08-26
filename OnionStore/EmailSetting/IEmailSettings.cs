namespace API.EmailSetting
{
    public interface IEmailSettings
    {
        public Task SendEmailMessage(EmailResponse email);
    }
}