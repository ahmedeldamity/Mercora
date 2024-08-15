using Core.Entities;

namespace Core.Interfaces.Services
{
    public interface IBrandService
    {
        Task<IReadOnlyList<ProductBrand>> GetBrandsAsync();
    }
}
