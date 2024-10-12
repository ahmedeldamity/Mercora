using BlazorEcommerce.Application.Dtos;
using BlazorEcommerce.Application.Specifications.ProductSpecifications;
using BlazorEcommerce.Domain.ErrorHandling;
using BlazorEcommerce.Shared.Product;

namespace BlazorEcommerce.Application.Interfaces.Services;
public interface IProductService
{
    Task<Result<PaginationToReturn<ProductResponse>>> GetProductsAsync(ProductSpecificationParameters specParams);
    Task<Result<ProductResponse>> GetProductAsync(int id);
    Task<Result<ProductResponse>> CreateProductAsync(ProductRequest productRequest);
    Task<Result<ProductResponse>> UpdateProductAsync(int id, ProductRequest productRequest);
    Task<Result<ProductResponse>> DeleteProductAsync(int id);
}