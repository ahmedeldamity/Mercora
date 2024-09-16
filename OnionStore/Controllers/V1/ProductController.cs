using API.Extensions;
using API.Helpers;
using Core.Dtos;
using Core.Interfaces.Services;
using Core.Specifications.ProductSpecifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V1;
public class ProductController(IProductService productService) : BaseController
{
    [HttpGet]
    [Cached(600)]
    public async Task<ActionResult> GetProducts([FromQuery] ProductSpecificationParameters specParams)
    {
        var result = await productService.GetProductsAsync(specParams);

        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    [Cached(600)]
    public async Task<ActionResult> GetProduct(int id)
    {
        var result = await productService.GetProductAsync(id);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost]
    public async Task<ActionResult> CreateProduct(ProductRequest productRequest)
    {
        var result = await productService.CreateProductAsync(productRequest);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, ProductRequest productRequest)
    {
        var result = await productService.UpdateProductAsync(id, productRequest);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var result = await productService.DeleteProductAsync(id);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

}