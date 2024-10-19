namespace BlazorEcommerce.Application.Interfaces.Services;
public interface IBrandService
{
    Task<Result<IReadOnlyList<ProductBrandResponse>>> GetBrandsAsync();
    Task<Result<IReadOnlyList<ProductBrandResponse>>> SearchBrandsAsync(string search);
    Task<Result<ProductBrandResponse>> GetBrandByIdAsync(int id);
    Task<Result<ProductBrandResponse>> CreateBrandAsync(ProductBrandRequest brandRequest);
    Task<Result<ProductBrandResponse>> UpdateBrandAsync(int id, ProductBrandRequest brandRequest);
    Task<Result<ProductBrandResponse>> DeleteBrandAsync(int id);
}