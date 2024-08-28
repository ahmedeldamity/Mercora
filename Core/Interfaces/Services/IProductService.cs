using Core.Dtos;
using Core.ErrorHandling;
using Core.Specifications.ProductSpecifications;
using Shared.Helpers;

namespace Core.Interfaces.Services;
public interface IProductService
{
    Task<Result<PaginationToReturn<ProductResponse>>> GetProductsAsync(ProductSpecificationParameters specParams);
    Task<Result<ProductResponse>> GetProductAsync(int id);
}