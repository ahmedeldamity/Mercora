using API.Extensions;
using API.Helpers;
using Core.Dtos;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V1;
public class BrandController(IBrandService _brandService) : BaseController
{
    [HttpGet]
    [Cached(600)]
    public async Task<ActionResult> GetBrands()
    {
        var result = await _brandService.GetBrandsAsync();

        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetBrandById(int id)
    {
        var result = await _brandService.GetBrandByIdAsync(id);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("search")]
    public async Task<ActionResult> SearchBrands(string search)
    {
        var result = await _brandService.SearchBrandsAsync(search);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult> CreateBrand(ProductBrandRequest brandRequest)
    {
        var result = await _brandService.CreateBrandAsync(brandRequest);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateBrand(int id, ProductBrandRequest brandRequest)
    {
        var result = await _brandService.UpdateBrandAsync(id, brandRequest);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBrand(int id)
    {
        var result = await _brandService.DeleteBrandAsync(id);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

}