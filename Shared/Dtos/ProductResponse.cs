namespace Shared.Dtos;
public class ProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string ImageCover { get; set; } = null!;
    public string[] Images { get; set; } = [];
    public decimal Quantity { get; set; }
    public decimal RatingsAverage { get; set; }
    public decimal Price { get; set; }
    public int BrandId { get; set; }
    public string Brand { get; set; } = null!;
    public int CategoryId { get; set; }
    public string Category { get; set; } = null!;
}