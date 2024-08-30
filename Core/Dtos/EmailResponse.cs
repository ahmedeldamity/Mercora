using Microsoft.AspNetCore.Http;

namespace Core.Dtos;
public class EmailResponse
{
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;
    public string To { get; set; } = null!;
    public IList<IFormFile>? Attachments { get; set; }
}