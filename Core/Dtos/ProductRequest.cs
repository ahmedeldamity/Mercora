namespace Core.Dtos;
public record ProductRequest(
    int Id,
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