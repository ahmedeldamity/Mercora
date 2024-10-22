namespace BlazorEcommerce.Client.Services.CategoryService;
public class CategoryService(IHttpClientFactory _httpClientFactory) : ICategoryService
{
	private readonly HttpClient httpClient = _httpClientFactory.CreateClient("Auth");

	public async Task<List<CategoryResponse>> GetAllCategories()
	{
		var response = await httpClient.GetFromJsonAsync<List<CategoryResponse>>("api/Category");

		return response ?? [];
	}
}