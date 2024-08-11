namespace API.EmailSetting
{
    public interface IEmailSettings
    {
        public Task SendEmailMessage(EmailSettingDto email);
    }
}