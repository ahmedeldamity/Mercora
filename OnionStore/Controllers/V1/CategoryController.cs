using BlazorEcommerce.Application.Dtos;
using BlazorEcommerce.Application.Interfaces.Services;
using BlazorEcommerce.Server.Extensions;
using BlazorEcommerce.Server.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace BlazorEcommerce.Server.Controllers.V1;
public class CategoryController(ICategoryService categoryService) : BaseController
{
    [HttpGet]
    [Cached(600)]
    public async Task<ActionResult> GetCategories()
    {
        var result = await categoryService.GetCategoriesAsync();

        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetCategoryById(int id)
    {
        var result = await categoryService.GetCategoryByIdAsync(id);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("search")]
    public async Task<ActionResult> SearchCategories(string search)
    {
        var result = await categoryService.SearchCategoriesAsync(search);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult> CreateCategory(ProductCategoryRequest categoryRequest)
    {
        var result = await categoryService.CreateCategoryAsync(categoryRequest);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateCategory(int id, ProductCategoryRequest categoryRequest)
    {
        var result = await categoryService.UpdateCategoryAsync(id, categoryRequest);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteCategory(int id)
    {
        var result = await categoryService.DeleteCategoryAsync(id);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

}