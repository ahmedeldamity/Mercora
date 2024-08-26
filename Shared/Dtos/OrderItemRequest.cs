namespace Shared.Dtos;
public class OrderItemRequest
{
    public int Id { get; set; } // Id For Product As Arrange In List Of Items
    public int ProductId { get; set; }  // Id For Product In Database
    public string ProductName { get; set; } = null!;
    public string ImageCover { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}