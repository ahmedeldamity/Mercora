using AutoMapper;
using BlazorEcommerce.Application.Dtos;
using BlazorEcommerce.Application.Interfaces.Repositories;
using BlazorEcommerce.Application.Interfaces.Services;
using BlazorEcommerce.Application.Specifications;
using BlazorEcommerce.Domain.Entities.ProductEntities;
using BlazorEcommerce.Domain.ErrorHandling;
using BlazorEcommerce.Shared.Category;

namespace BlazorEcommerce.Infrastructure.Services;
public class CategoryService(IUnitOfWork unitOfWork, IMapper mapper) : ICategoryService
{
    public async Task<Result<IReadOnlyList<CategoryResponse>>> GetCategoriesAsync()
    {
        var categories = await unitOfWork.Repository<ProductCategory>().GetAllAsync();

        var categoriesResponse = mapper.Map<IReadOnlyList<ProductCategory>, IReadOnlyList<CategoryResponse>>(categories);

        return Result.Success(categoriesResponse);
    }

    public async Task<Result<CategoryResponse>> GetCategoryByIdAsync(int id)
    {
        var category = await unitOfWork.Repository<ProductCategory>().GetEntityAsync(id);

        if(category == null)
			return Result.Failure<CategoryResponse>(new Error(404, $"Category with id {id} not found"));

        var categoryResponse = mapper.Map<ProductCategory, CategoryResponse>(category);

        return categoryResponse == null ? Result.Failure<CategoryResponse>(new Error(404, $"Category with id {id} not found")) : Result.Success(categoryResponse);
    }

    public async Task<Result<IReadOnlyList<CategoryResponse>>> SearchCategoriesAsync(string search)
    {
        var spec = new BaseSpecifications<ProductCategory> { WhereCriteria = x => x.Name.Contains(search) };

        var categories = await unitOfWork.Repository<ProductCategory>().GetAllAsync(spec);

        var categoriesResponse = mapper.Map<IReadOnlyList<ProductCategory>, IReadOnlyList<CategoryResponse>>(categories);

        return Result.Success(categoriesResponse);
    }

    public async Task<Result<CategoryResponse>> CreateCategoryAsync(ProductCategoryRequest categoryRequest)
    {
        var category = mapper.Map<ProductCategoryRequest, ProductCategory>(categoryRequest);

        await unitOfWork.Repository<ProductCategory>().AddAsync(category);

        var result = await unitOfWork.CompleteAsync();

        if(result <= 0)
			return Result.Failure<CategoryResponse>(new Error(500, "Error occurred while saving category"));

        var categoryResponse = mapper.Map<ProductCategory, CategoryResponse>(category);

        return Result.Success(categoryResponse);
    }

    public async Task<Result<CategoryResponse>> UpdateCategoryAsync(int id, ProductCategoryRequest categoryRequest)
    {
        var category = await unitOfWork.Repository<ProductCategory>().GetEntityAsync(id);

        if (category == null)
            return Result.Failure<CategoryResponse>(new Error(404, $"Category with id {id} not found"));

        mapper.Map(categoryRequest, category);

        unitOfWork.Repository<ProductCategory>().Update(category);

        var result = await unitOfWork.CompleteAsync();

        if(result <= 0)
			return Result.Failure<CategoryResponse>(new Error(500, "Error occurred while updating category"));

        var categoryResponse = mapper.Map<ProductCategory, CategoryResponse>(category);

        return Result.Success(categoryResponse);
    }

    public async Task<Result<CategoryResponse>> DeleteCategoryAsync(int id)
    {
        var category = await unitOfWork.Repository<ProductCategory>().GetEntityAsync(id);

        if (category == null)
            return Result.Failure<CategoryResponse>(new Error(404, $"Category with id {id} not found"));

        unitOfWork.Repository<ProductCategory>().Delete(category);

        var result = await unitOfWork.CompleteAsync();

        if(result <= 0)
            return Result.Failure<CategoryResponse>(new Error(500, "Error occurred while deleting category"));

        var categoryResponse = mapper.Map<ProductCategory, CategoryResponse>(category);

        return Result.Success(categoryResponse);
    }

}