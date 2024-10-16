namespace BlazorEcommerce.Application.Specifications.ProductSpecifications;
public class ProductSpecificationParameters
{
    private const int MaxPageSize = 40;

    private int _pageSize = 9;
    public int PageIndex { get; set; } = 1;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? _pageSize : value;
    }
    public int? BrandId { get; set; }
    public int? CategoryId { get; set; }
    public string? Sort { get; set; }
    public string? Search { get; set; }
}