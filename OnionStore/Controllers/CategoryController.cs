using API.Helpers;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
public class CategoryController(ICategoryService _categoryService) : BaseController
{
    [HttpGet]
    [Cached(600)]
    public async Task<ActionResult> GetCategories()
    {
        var result = await _categoryService.GetCategoriesAsync();

        return Ok(result.Value);
    }

}