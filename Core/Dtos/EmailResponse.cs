namespace Core.Dtos;
public record EmailResponse(
    string Subject,
    string Body,
    string To
);