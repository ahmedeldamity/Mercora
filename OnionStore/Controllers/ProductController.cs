using API.Dtos;
using API.Errors;
using AutoMapper;
using Core.Entities;
using Core.Entities.Product_Entities;
using Core.Interfaces.Services;
using Core.Specifications.ProductSpecifications;
using DotNetCore_ECommerce.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProductController(IProductService _productService, IMapper _mapper) : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts([FromQuery] ProductSpecificationParameters specParams)
        {
            var products = await _productService.GetProductsAsync(specParams);

            var productsDto = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

            return Ok(productsDto);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProduct(int id)
        {
            var product = await _productService.GetProductAsync(id);

            if (product is null)
                return NotFound(new ApiResponse(404));

            return Ok(_mapper.Map<Product, ProductToReturnDto>(product));
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrandToReturnDto>>> GetBrands()
        {
            var brands = await _productService.GetBrandsAsync();
            var brandsDto = _mapper.Map<IReadOnlyList<ProductBrand>, IReadOnlyList<ProductBrandToReturnDto>>(brands);
            return Ok(brandsDto);
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetCategories()
        {
            var categories = await _productService.GetCategoriesAsync();
            return Ok(categories);
        }
    }
}
