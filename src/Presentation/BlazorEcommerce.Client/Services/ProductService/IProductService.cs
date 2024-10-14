using BlazorEcommerce.Shared.Product;
using BlazorEcommerce.Shared.Specifications.ProductSpecifications;

namespace BlazorEcommerce.Client.Services.ProductService;
public interface IProductService
{
	public string Message { get; set; }
	Task<PaginationToReturn<ProductResponse>?> GetProductsAsync(ProductParameters specParams);
	Task<List<ProductResponse>> GetFeaturedProductsAsync();
	Task<ProductResponse?> GetProductAsync(int id);
	Task<List<string>> GetProductSearchSuggestions(string searchText);
}