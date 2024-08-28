using Shared.Dtos;
using Shared.Helpers;

namespace Core.Interfaces.Services;
public interface IBrandService
{
    Task<Result<IReadOnlyList<ProductBrandResponse>>> GetBrandsAsync();
}