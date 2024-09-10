namespace Core.Dtos;
public record OrderItemRequest(
    int Id,
    int ProductId,
    string ProductName,
    string ImageCover,
    decimal Price,
    int Quantity
);