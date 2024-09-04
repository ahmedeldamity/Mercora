namespace Core.Dtos;
public record ProductRequest
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
    public int CategoryId { get; set; }
}