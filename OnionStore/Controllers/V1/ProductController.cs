using API.Extensions;
using API.Helpers;
using Core.Dtos;
using Core.Interfaces.Services;
using Core.Specifications.ProductSpecifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V1;
public class ProductController(IProductService _productService) : BaseController
{
    [HttpGet]
    [Cached(600)]
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

    [HttpPost]
    public async Task<ActionResult> CreateProduct(ProductRequest productRequest)
    {
        var result = await _productService.CreateProductAsync(productRequest);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProduct(int id, ProductRequest productRequest)
    {
        var result = await _productService.UpdateProductAsync(id, productRequest);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var result = await _productService.DeleteProductAsync(id);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

}