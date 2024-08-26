using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;

namespace Service;
public class CategoryService(IUnitOfWork _unitOfWork): ICategoryService
{
    public async Task<IReadOnlyList<ProductCategory>> GetCategoriesAsync()
    {
        var categories = await _unitOfWork.Repository<ProductCategory>().GetAllAsync();
        return categories;
    }
}