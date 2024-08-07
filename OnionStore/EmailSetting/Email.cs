namespace API.EmailSetting
{
    public class Email
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string To { get; set; }
        public IList<IFormFile>? Attachments { get; set; }
    }
}