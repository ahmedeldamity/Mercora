using Core.Entities.Product_Entities;
using Core.Specifications.ProductSpecifications;
using Shared.Dtos;
using Shared.Helpers;

namespace Core.Interfaces.Services;
public interface IProductService
{
    Task<Result<PaginationToReturn<ProductResponse>>> GetProductsAsync(ProductSpecificationParameters specParams);
    Task<Result<ProductResponse>> GetProductAsync(int id);
}