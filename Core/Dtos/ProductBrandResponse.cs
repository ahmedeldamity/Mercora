namespace Core.Dtos;
public record ProductBrandResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string ImageCover { get; set; } = null!;
}