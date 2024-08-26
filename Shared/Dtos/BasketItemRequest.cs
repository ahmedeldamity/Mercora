namespace Shared.Dtos;
public class BasketItemRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public string ImageCover { get; set; } = null!;
    public string[] Images { get; set; } = [];
    public decimal Quantity { get; set; }
    public decimal RatingsAverage { get; set; }
    public string Category { get; set; } = null!;
    public string Brand { get; set; } = null!;
}