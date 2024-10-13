using BlazorEcommerce.Application.Dtos;
using BlazorEcommerce.Application.Interfaces.Services;
using BlazorEcommerce.Application.Specifications.ProductSpecifications;
using BlazorEcommerce.Server.Extensions;
using BlazorEcommerce.Server.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace BlazorEcommerce.Server.Controllers.V1;
public class ProductController(IProductService productService) : BaseController
{
    [HttpGet]
    [Cached(600)]
    public async Task<ActionResult> GetProducts([FromQuery] ProductSpecificationParameters specParams)
    {
        var result = await productService.GetProductsAsync(specParams);

        return Ok(result.Value);
    }

    [HttpGet("featured")]
    [Cached(600)]
    public async Task<ActionResult> GetFeaturedProducts()
    {
	    var result = await productService.GetFeaturedProductsAsync();

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

    [HttpGet("SearchSuggestions/{searchText}")]
    public async Task<ActionResult> GetProductSearchSuggestions(string searchText)
	{
		var result = await productService.GetProductSearchSuggestions(searchText);

		return Ok(result);
	}

}