using BlazorEcommerce.Application.Dtos;
using BlazorEcommerce.Domain.Entities.ProductEntities;
using BlazorEcommerce.Domain.ErrorHandling;

namespace BlazorEcommerce.Application.Interfaces.Services;
public interface ICategoryService
{
    Task<Result<IReadOnlyList<ProductCategory>>> GetCategoriesAsync();
    Task<Result<ProductCategory>> GetCategoryByIdAsync(int id);
    Task<Result<IReadOnlyList<ProductCategory>>> SearchCategoriesAsync(string search);
    Task<Result<ProductCategory>> CreateCategoryAsync(ProductCategoryRequest categoryRequest);
    Task<Result<ProductCategory>> UpdateCategoryAsync(int id, ProductCategoryRequest categoryRequest);
    Task<Result<ProductCategory>> DeleteCategoryAsync(int id);
}