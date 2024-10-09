namespace BlazorEcommerce.Application.Dtos;
public record BasketItemResponse(
    int Id,
    string Name,
    string Description,
    decimal Price,
    string ImageCover,
    string[] Images,
    decimal Quantity,
    decimal RatingsAverage,
    string Category,
    string Brand
);