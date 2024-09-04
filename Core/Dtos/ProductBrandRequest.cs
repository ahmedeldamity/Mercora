namespace Core.Dtos;
public record ProductBrandRequest
{
    public string Name { get; set; } = null!;
    public string ImageCover { get; set; } = null!;
}