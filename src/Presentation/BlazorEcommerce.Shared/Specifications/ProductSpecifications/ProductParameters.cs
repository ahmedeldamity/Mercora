namespace BlazorEcommerce.Shared.Specifications.ProductSpecifications;
public class ProductParameters
{
    public int PageIndex { get; set; } = 1;
    public string? Sort { get; set; } = "name";
    public int BrandId { get; set; }
    public int CategoryId { get; set; }
    public string? Search { get; set; }
}