using System.Net.Http.Json;
using BlazorEcommerce.Shared.Product;
using BlazorEcommerce.Shared.Specifications.ProductSpecifications;

namespace BlazorEcommerce.Client.Services.ProductService;
public class ProductService(HttpClient httpClient) : IProductService
{
	public async Task<PaginationToReturn<ProductResponse>?> GetProductsAsync(ProductParameters specParams)
	{
		var response = await httpClient.GetFromJsonAsync<PaginationToReturn<ProductResponse>>("api/Product");

		return response;
	}
}