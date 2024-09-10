namespace Core.Dtos;
public record OrderItemRequest(
    int Id, // Id For Product As Arrange In List Of Items
    int ProductId, // Id For Product In Database
    string ProductName,
    string ImageCover,
    decimal Price,
    int Quantity
);