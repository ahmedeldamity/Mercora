using API.Errors;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Entities.Product_Entities;
using Core.Interfaces.Services;
using Core.Specifications.ProductSpecifications;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;

namespace API.Controllers;
public class ProductController(IProductService _productService, ICategoryService _categoryService,
             IBrandService _brandService ,IMapper _mapper) : BaseController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts([FromQuery] ProductSpecificationParameters specParams)
    {
        var products = await _productService.GetProductsAsync(specParams);

        var productsDto = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductResponse>>(products);

        var productsCount = await _productService.GetProductCount(specParams);

        return Ok(new PaginationToReturn<ProductResponse>(specParams.PageIndex, specParams.PageSize, productsCount, productsDto));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetProduct(int id)
    {
        var product = await _productService.GetProductAsync(id);

        if (product is null)
            return NotFound(new ApiResponse(404));

        return Ok(_mapper.Map<Product, ProductResponse>(product));
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<ProductBrandResponse>>> GetBrands()
    {
        var brands = await _brandService.GetBrandsAsync();
        var brandsDto = _mapper.Map<IReadOnlyList<ProductBrand>, IReadOnlyList<ProductBrandResponse>>(brands);
        return Ok(brandsDto);
    }

    [HttpGet("categories")]
    public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetCategories()
    {
        var categories = await _categoryService.GetCategoriesAsync();
        return Ok(categories);
    }
}