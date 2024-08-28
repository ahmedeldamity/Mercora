using AutoMapper;
using Core.Dtos;
using Core.Entities.Product_Entities;
using Core.ErrorHandling;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Specifications.ProductSpecifications;
using Shared.Helpers;

namespace Service;
public class ProductService(IUnitOfWork _unitOfWork, IMapper _mapper) : IProductService
{
    public async Task<Result<PaginationToReturn<ProductResponse>>> GetProductsAsync(ProductSpecificationParameters specParams)
    {
        var spec = new ProductWithBrandAndCategorySpecifications(specParams);

        var products = await _unitOfWork.Repository<Product>().GetAllAsync(spec);

        var productsDto = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductResponse>>(products);

        var productsCount = await GetProductCount(specParams);

        var productsWithPagination = new PaginationToReturn<ProductResponse>(specParams.PageIndex, specParams.PageSize, productsCount, productsDto);

        return Result.Success(productsWithPagination);
    }

    private async Task<int> GetProductCount(ProductSpecificationParameters specParams)
    {
        var spec = new ProductCountSpecification(specParams);

        var productsCount = await _unitOfWork.Repository<Product>().GetCountAsync(spec);

        return productsCount;
    }

    public async Task<Result<ProductResponse>> GetProductAsync(int id)
    {
        var spec = new ProductWithBrandAndCategorySpecifications(id);

        var product = await _unitOfWork.Repository<Product>().GetEntityAsync(spec);

        if (product is null)
            return Result.Failure<ProductResponse>(new Error("Product not found", 404));

        var productDto = _mapper.Map<Product, ProductResponse>(product);

        return Result.Success(productDto);
    }
}