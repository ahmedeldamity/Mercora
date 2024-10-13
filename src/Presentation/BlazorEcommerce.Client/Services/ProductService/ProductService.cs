using BlazorEcommerce.Shared.Product;
using BlazorEcommerce.Shared.Specifications.ProductSpecifications;
using System.Net.Http.Json;

namespace BlazorEcommerce.Client.Services.ProductService;
public class ProductService(HttpClient httpClient) : IProductService
{
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

	public async Task<List<string>> GetProductSearchSuggestions(string searchText)
	{
		var response = await httpClient.GetFromJsonAsync<List<string>>($"api/Product/SearchSuggestions/{searchText}");

		return response ?? [];
	}
}