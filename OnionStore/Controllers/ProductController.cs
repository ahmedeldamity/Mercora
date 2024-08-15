using AutoMapper;
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
    }
}
