using Core.Entities;
using Core.ErrorHandling;
using Shared.Helpers;

namespace Core.Interfaces.Services;
public interface ICategoryService
{
    Task<Result<IReadOnlyList<ProductCategory>>> GetCategoriesAsync();
}