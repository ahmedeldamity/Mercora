using API.Extensions;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
public class CategoryController(ICategoryService _categoryService) : BaseController
{
    [HttpGet]
    public async Task<ActionResult> GetCategories()
    {
        var result = await _categoryService.GetCategoriesAsync();

        return result.ToSuccess();
    }

}