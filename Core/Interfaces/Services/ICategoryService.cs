using Core.Entities;
using Core.ErrorHandling;

namespace Core.Interfaces.Services;
public interface ICategoryService
{
    Task<Result<IReadOnlyList<ProductCategory>>> GetCategoriesAsync();
}