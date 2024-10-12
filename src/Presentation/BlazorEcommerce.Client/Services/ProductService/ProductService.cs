using System.Net.Http.Json;
using BlazorEcommerce.Shared.Product;
using BlazorEcommerce.Shared.Specifications.ProductSpecifications;

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
}