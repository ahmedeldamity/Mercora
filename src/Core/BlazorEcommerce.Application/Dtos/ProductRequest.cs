namespace BlazorEcommerce.Application.Dtos;
public record ProductRequest(
    string Name,
    string Description,
    string ImageCover,
    string[] Images,
    decimal Quantity,
    decimal RatingsAverage,
    decimal Price,
    int BrandId,
    int CategoryId
);