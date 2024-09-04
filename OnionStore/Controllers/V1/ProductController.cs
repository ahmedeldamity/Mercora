using API.Extensions;
using API.Helpers;
using Core.Interfaces.Services;
using Core.Specifications.ProductSpecifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V1;
public class ProductController(IProductService _productService) : BaseController
{
    [HttpGet]
    //[Cached(600)]
    public async Task<ActionResult> GetProducts([FromQuery] ProductSpecificationParameters specParams)
    {
        var result = await _productService.GetProductsAsync(specParams);

        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    [Cached(600)]
    public async Task<ActionResult> GetProduct(int id)
    {
        var result = await _productService.GetProductAsync(id);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

}