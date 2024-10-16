namespace BlazorEcommerce.Application.Dtos;
public record ProductRequest(
    string Name,
    string Description,
    string ImageCover,
    decimal Quantity,
    decimal RatingsAverage,
    decimal Price,
    int BrandId,
    int CategoryId
);