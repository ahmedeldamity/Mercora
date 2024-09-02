using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V1;
public class BrandController(IBrandService _brandService) : BaseController
{
    [HttpGet]
    public async Task<ActionResult> GetBrands()
    {
        var result = await _brandService.GetBrandsAsync();

        return Ok(result.Value);
    }

}