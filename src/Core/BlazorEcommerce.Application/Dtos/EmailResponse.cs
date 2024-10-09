namespace BlazorEcommerce.Application.Dtos;
public record EmailResponse(
    string Subject,
    string Body,
    string To
);