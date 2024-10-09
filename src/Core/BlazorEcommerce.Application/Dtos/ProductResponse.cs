namespace BlazorEcommerce.Application.Dtos;
public record ProductResponse(
    int Id,
    string Name,
    string Description,
    string ImageCover,
    string[] Images,
    decimal Quantity,
    decimal RatingsAverage,
    decimal Price,
    int BrandId,
    string Brand,
    int CategoryId,
    string Category  
);