namespace BlazorEcommerce.Application.Interfaces.Services;
public interface ICategoryService
{
    Task<Result<IReadOnlyList<CategoryResponse>>> GetCategoriesAsync();
    Task<Result<CategoryResponse>> GetCategoryByIdAsync(int id);
    Task<Result<IReadOnlyList<CategoryResponse>>> SearchCategoriesAsync(string search);
    Task<Result<CategoryResponse>> CreateCategoryAsync(ProductCategoryRequest categoryRequest);
    Task<Result<CategoryResponse>> UpdateCategoryAsync(int id, ProductCategoryRequest categoryRequest);
    Task<Result<CategoryResponse>> DeleteCategoryAsync(int id);
}