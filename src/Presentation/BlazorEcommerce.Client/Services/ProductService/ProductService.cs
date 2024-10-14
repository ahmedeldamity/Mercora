using BlazorEcommerce.Shared.Product;
using BlazorEcommerce.Shared.Specifications.ProductSpecifications;
using System.Net.Http.Json;
using BlazorEcommerce.Shared.Response;

namespace BlazorEcommerce.Client.Services.ProductService;
public class ProductService(HttpClient httpClient) : IProductService
{
	public string Message { get; set; } = "Loading products...";

	public async Task<PaginationToReturn<ProductResponse>?> GetProductsAsync(ProductParameters specParams)
	{
		var uri = "api/Product?pageIndex=" + specParams.PageIndex;

		if (specParams.BrandId.HasValue)
			uri += "&brandId=" + specParams.BrandId;

		if (specParams.CategoryId.HasValue)
			uri += "&categoryId=" + specParams.CategoryId;

		if (!string.IsNullOrWhiteSpace(specParams.Sort))
			uri += "&sort=" + specParams.Sort;

		if (!string.IsNullOrWhiteSpace(specParams.Search))
			uri += "&search=" + specParams.Search;

		var response = await httpClient.GetFromJsonAsync<PaginationToReturn<ProductResponse>>(uri);

		return response;
	}

	public async Task<List<ProductResponse>> GetFeaturedProductsAsync()
	{
		var response = await httpClient.GetFromJsonAsync<List<ProductResponse>>("api/Product/featured");

		return response ?? [];
	}

	public async Task<ProductResponse?> GetProductAsync(int id)
	{
		var response = await httpClient.GetAsync($"api/Product/{id}");

		Console.WriteLine($"Status: {response.IsSuccessStatusCode}");

		if (response.IsSuccessStatusCode is false)
		{
			var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();

			Message = errorResponse?.Message ?? "Product not fount";

			return null;
		}

		var product = await response.Content.ReadFromJsonAsync<ProductResponse>();

		return product;
	}

	public async Task<List<string>> GetProductSearchSuggestions(string searchText)
	{
		var response = await httpClient.GetFromJsonAsync<List<string>>($"api/Product/SearchSuggestions/{searchText}");

		return response ?? [];
	}
}