using AutoMapper;
using Core.Dtos;
using Core.Entities.ProductEntities;
using Core.ErrorHandling;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Specifications;

namespace Service;
public class CategoryService(IUnitOfWork unitOfWork, IMapper mapper): ICategoryService
{
    public async Task<Result<IReadOnlyList<ProductCategory>>> GetCategoriesAsync()
    {
        var categories = await unitOfWork.Repository<ProductCategory>().GetAllAsync();

        return Result.Success(categories);
    }

    public async Task<Result<ProductCategory>> GetCategoryByIdAsync(int id)
    {
        var category = await unitOfWork.Repository<ProductCategory>().GetEntityAsync(id);

        return category == null ? Result.Failure<ProductCategory>(new Error(404, $"Category with id {id} not found")) : Result.Success(category);
    }

    public async Task<Result<IReadOnlyList<ProductCategory>>> SearchCategoriesAsync(string search)
    {
        var spec = new BaseSpecifications<ProductCategory>() { WhereCriteria = x => x.Name.Contains(search) };

        var categories = await unitOfWork.Repository<ProductCategory>().GetAllAsync(spec);

        return Result.Success(categories);
    }

    public async Task<Result<ProductCategory>> CreateCategoryAsync(ProductCategoryRequest categoryRequest)
    {
        var category = mapper.Map<ProductCategoryRequest, ProductCategory>(categoryRequest);

        await unitOfWork.Repository<ProductCategory>().AddAsync(category);

        var result = await unitOfWork.CompleteAsync();

        return result <= 0 ? Result.Failure<ProductCategory>(new Error(500, "Error occurred while saving category")) : Result.Success(category);
    }

    public async Task<Result<ProductCategory>> UpdateCategoryAsync(int id, ProductCategoryRequest categoryRequest)
    {
        var category = await unitOfWork.Repository<ProductCategory>().GetEntityAsync(id);

        if (category == null)
            return Result.Failure<ProductCategory>(new Error(404, $"Category with id {id} not found"));

        mapper.Map(categoryRequest, category);

        unitOfWork.Repository<ProductCategory>().Update(category);

        var result = await unitOfWork.CompleteAsync();

        return result <= 0 ? Result.Failure<ProductCategory>(new Error(500, "Error occurred while updating category")) : Result.Success(category);
    }

    public async Task<Result<ProductCategory>> DeleteCategoryAsync(int id)
    {
        var category = await unitOfWork.Repository<ProductCategory>().GetEntityAsync(id);

        if (category == null)
            return Result.Failure<ProductCategory>(new Error(404, $"Category with id {id} not found"));

        unitOfWork.Repository<ProductCategory>().Delete(category);

        var result = await unitOfWork.CompleteAsync();

        return result <= 0 ? Result.Failure<ProductCategory>(new Error(500, "Error occurred while deleting category")) : Result.Success(category);
    }    

}