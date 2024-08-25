namespace Core.Entities.OrderEntities;
public class ProductOrderItem
{
    public ProductOrderItem() { /* we create this constractor because EF need it while migration to make instance from this class */ }
    public ProductOrderItem(int productId, string productName, string imageCover)
    {
        ProductId = productId;
        ProductName = productName;
        ProductImageCover = imageCover;
    }

    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string ProductImageCover { get; set; } = null!;
}