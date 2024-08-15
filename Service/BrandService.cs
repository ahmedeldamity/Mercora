using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;

namespace Service
{
    public class BrandService(IUnitOfWork _unitOfWork): IBrandService
    {
        public async Task<IReadOnlyList<ProductBrand>> GetBrandsAsync()
        {
            var brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();
            return brands;
        }
    }
}
