using API.Helpers;
using Asp.Versioning;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V1;
public class CategoryController(ICategoryService _categoryService) : BaseController
{
    [HttpGet]
    [ApiVersion("2.0")]
    [Cached(600)]
    public async Task<ActionResult> GetCategories()
    {
        var result = await _categoryService.GetCategoriesAsync();

        return Ok(result.Value);
    }

}