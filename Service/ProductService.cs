using AutoMapper;
using Core.Dtos;
using Core.Entities;
using Core.Entities.Product_Entities;
using Core.ErrorHandling;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Specifications.ProductSpecifications;

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
            return Result.Failure<ProductResponse>(new Error(404, "The product you are looking for does not exist. Please check the product ID and try again."));

        var productDto = _mapper.Map<Product, ProductResponse>(product);

        return Result.Success(productDto);
    }

    public async Task<Result<ProductResponse>> CreateProductAsync(ProductRequest productRequest)
    {
        var product = _mapper.Map<ProductRequest, Product>(productRequest);

        var productBrand = await _unitOfWork.Repository<ProductBrand>().GetEntityAsync(productRequest.BrandId);

        if (productBrand is null)
            return Result.Failure<ProductResponse>(new Error(404, "The brand you are looking for does not exist. Please check the brand ID and try again."));

        var productCategory = await _unitOfWork.Repository<ProductCategory>().GetEntityAsync(productRequest.CategoryId);

        if (productCategory is null)
            return Result.Failure<ProductResponse>(new Error(404, "The category you are looking for does not exist. Please check the category ID and try again."));

        product.Brand = productBrand;
        product.Category = productCategory;


        await _unitOfWork.Repository<Product>().AddAsync(product);

        var result = await _unitOfWork.CompleteAsync();

        if (result <= 0)
            return Result.Failure<ProductResponse>(new Error(500, "An error occurred while creating the product. Please try again."));

        var productDto = _mapper.Map<Product, ProductResponse>(product);

        return Result.Success(productDto);
    }

    public async Task<Result<ProductResponse>> UpdateProductAsync(int id, ProductRequest productRequest)
    {
        var product = await _unitOfWork.Repository<Product>().GetEntityAsync(id);

        if (product is null)
            return Result.Failure<ProductResponse>(new Error(404, "The product you are looking for does not exist. Please check the product ID and try again."));

        var productBrand = await _unitOfWork.Repository<ProductBrand>().GetEntityAsync(productRequest.BrandId);

        if (productBrand is null)
            return Result.Failure<ProductResponse>(new Error(404, "The brand you are looking for does not exist. Please check the brand ID and try again."));

        var productCategory = await _unitOfWork.Repository<ProductCategory>().GetEntityAsync(productRequest.CategoryId);

        if (productCategory is null)
            return Result.Failure<ProductResponse>(new Error(404, "The category you are looking for does not exist. Please check the category ID and try again."));

        var newProduct = _mapper.Map<ProductRequest, Product>(productRequest);

        _unitOfWork.Repository<Product>().Update(newProduct);

        var result = await _unitOfWork.CompleteAsync();

        if (result <= 0)
            return Result.Failure<ProductResponse>(new Error(500, "An error occurred while updating the product. Please try again."));

        var productDto = _mapper.Map<Product, ProductResponse>(newProduct);

        return Result.Success(productDto);
    }

    public async Task<Result<ProductResponse>> DeleteProductAsync(int id)
    {
        var productResp = _unitOfWork.Repository<Product>();

        var product = await productResp.GetEntityAsync(id);

        if (product is null)
            return Result.Failure<ProductResponse>(new Error(404, "The product you are looking for does not exist. Please check the product ID and try again."));

        productResp.Delete(product);

        var result = await _unitOfWork.CompleteAsync();

        if (result <= 0)
            return Result.Failure<ProductResponse>(new Error(500, "An error occurred while deleting the product. Please try again."));

        var productDto = _mapper.Map<Product, ProductResponse>(product);

        return Result.Success(productDto);
    }

}