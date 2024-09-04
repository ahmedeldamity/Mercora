using AutoMapper;
using Core.Dtos;
using Core.Entities;
using Core.ErrorHandling;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Specifications;

namespace Service;
public class CategoryService(IUnitOfWork _unitOfWork, IMapper _mapper): ICategoryService
{
    public async Task<Result<IReadOnlyList<ProductCategory>>> GetCategoriesAsync()
    {
        var categories = await _unitOfWork.Repository<ProductCategory>().GetAllAsync();

        return Result.Success(categories);
    }

    public async Task<Result<ProductCategory>> GetCategoryByIdAsync(int id)
    {
        var category = await _unitOfWork.Repository<ProductCategory>().GetEntityAsync(id);

        if (category == null)
            return Result.Failure<ProductCategory>(new Error(404, $"Category with id {id} not found"));

        return Result.Success(category);
    }

    public async Task<Result<IReadOnlyList<ProductCategory>>> SearchCategoriesAsync(string search)
    {
        var spec = new BaseSpecifications<ProductCategory>() { WhereCriteria = x => x.Name.Contains(search) };

        var categories = await _unitOfWork.Repository<ProductCategory>().GetAllAsync(spec);

        return Result.Success(categories);
    }

    public async Task<Result<ProductCategory>> CreateCategoryAsync(ProductCategoryRequest categoryRequest)
    {
        var category = _mapper.Map<ProductCategoryRequest, ProductCategory>(categoryRequest);

        await _unitOfWork.Repository<ProductCategory>().AddAsync(category);

        var result = await _unitOfWork.CompleteAsync();

        if (result <= 0)
            return Result.Failure<ProductCategory>(new Error(500, "Error occurred while saving category"));

        return Result.Success(category);
    }

    public async Task<Result<ProductCategory>> UpdateCategoryAsync(int id, ProductCategoryRequest categoryRequest)
    {
        var category = await _unitOfWork.Repository<ProductCategory>().GetEntityAsync(id);

        if (category == null)
            return Result.Failure<ProductCategory>(new Error(404, $"Category with id {id} not found"));

        _mapper.Map(categoryRequest, category);

        _unitOfWork.Repository<ProductCategory>().Update(category);

        var result = await _unitOfWork.CompleteAsync();

        if (result <= 0)
            return Result.Failure<ProductCategory>(new Error(500, "Error occurred while updating category"));

        return Result.Success(category);
    }

    public async Task<Result<ProductCategory>> DeleteCategoryAsync(int id)
    {
        var category = await _unitOfWork.Repository<ProductCategory>().GetEntityAsync(id);

        if (category == null)
            return Result.Failure<ProductCategory>(new Error(404, $"Category with id {id} not found"));

        _unitOfWork.Repository<ProductCategory>().Delete(category);

        var result = await _unitOfWork.CompleteAsync();

        if (result <= 0)
            return Result.Failure<ProductCategory>(new Error(500, "Error occurred while deleting category"));

        return Result.Success(category);
    }    

}