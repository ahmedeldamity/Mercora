using Core.Entities;

namespace Core.Interfaces.Services;
public interface ICategoryService
{
    Task<IReadOnlyList<ProductCategory>> GetCategoriesAsync();
}