namespace BlazorEcommerce.Client.Services.CategoryService;
public class CategoryService(HttpClient httpClient): ICategoryService
{
	public async Task<List<CategoryResponse>> GetAllCategories()
	{
		var response = await httpClient.GetFromJsonAsync<List<CategoryResponse>>("api/Category");

		return response ?? [];
	}
}