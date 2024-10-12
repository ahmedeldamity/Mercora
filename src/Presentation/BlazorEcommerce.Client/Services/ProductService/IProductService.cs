using BlazorEcommerce.Shared.Product;
using BlazorEcommerce.Shared.Specifications.ProductSpecifications;

namespace BlazorEcommerce.Client.Services.ProductService;
public interface IProductService
{
	Task<PaginationToReturn<ProductResponse>?> GetProductsAsync(ProductParameters specParams);
}