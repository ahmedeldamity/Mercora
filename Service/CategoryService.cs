using Core.Entities;
using Shared.Helpers;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;

namespace Service;
public class CategoryService(IUnitOfWork _unitOfWork): ICategoryService
{
    public async Task<Result<IReadOnlyList<ProductCategory>>> GetCategoriesAsync()
    {
        var categories = await _unitOfWork.Repository<ProductCategory>().GetAllAsync();

        return Result.Success(categories);
    }
}