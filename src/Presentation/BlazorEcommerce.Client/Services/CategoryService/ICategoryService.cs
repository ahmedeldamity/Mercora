using BlazorEcommerce.Shared.Category;

namespace BlazorEcommerce.Client.Services.CategoryService;
public interface ICategoryService
{
	Task<List<CategoryResponse>> GetAllCategories();
}