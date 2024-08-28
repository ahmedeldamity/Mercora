using Core.Dtos;
using Core.ErrorHandling;

namespace Core.Interfaces.Services;
public interface IBrandService
{
    Task<Result<IReadOnlyList<ProductBrandResponse>>> GetBrandsAsync();
}