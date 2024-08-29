using Microsoft.AspNetCore.Mvc;
using Core.Interfaces.Services;
using API.Extensions;

namespace API.Controllers;
public class BrandController(IBrandService _brandService) : BaseController
{
    [HttpGet]
    public async Task<ActionResult> GetBrands()
    {
        var result = await _brandService.GetBrandsAsync();

        return result.ToSuccess();
    }

}