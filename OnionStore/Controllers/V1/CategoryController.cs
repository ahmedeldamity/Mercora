using API.Extensions;
using API.Helpers;
using Core.Dtos;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V1;
public class CategoryController(ICategoryService _categoryService) : BaseController
{
    [HttpGet]
    [Cached(600)]
    public async Task<ActionResult> GetCategories()
    {
        var result = await _categoryService.GetCategoriesAsync();

        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetCategoryById(int id)
    {
        var result = await _categoryService.GetCategoryByIdAsync(id);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("search")]
    public async Task<ActionResult> SearchCategories(string search)
    {
        var result = await _categoryService.SearchCategoriesAsync(search);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult> CreateCategory(ProductCategoryRequest categoryRequest)
    {
        var result = await _categoryService.CreateCategoryAsync(categoryRequest);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCategory(int id, ProductCategoryRequest categoryRequest)
    {
        var result = await _categoryService.UpdateCategoryAsync(id, categoryRequest);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategory(int id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

}