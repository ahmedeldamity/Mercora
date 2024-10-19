namespace BlazorEcommerce.Shared.Product;
public record ProductResponse(
    int Id,
    string Name,
    string Description,
    string ImageCover,
    decimal Quantity,
    decimal RatingsAverage,
    decimal Price,
    int BrandId,
    string BrandName,
    int CategoryId,
    string CategoryName
);