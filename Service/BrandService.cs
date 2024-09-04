using AutoMapper;
using Core.Dtos;
using Core.Entities;
using Core.ErrorHandling;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Specifications;

namespace Service;
public class BrandService(IUnitOfWork _unitOfWork, IMapper _mapper): IBrandService
{
    public async Task<Result<IReadOnlyList<ProductBrandResponse>>> GetBrandsAsync()
    {
        var brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();

        var brandsDto = _mapper.Map<IReadOnlyList<ProductBrand>, IReadOnlyList<ProductBrandResponse>>(brands);

        return Result.Success(brandsDto);
    }

    public async Task<Result<ProductBrandResponse>> GetBrandByIdAsync(int id)
    {
        var brand = await _unitOfWork.Repository<ProductBrand>().GetEntityAsync(id);

        if (brand == null)
            return Result.Failure<ProductBrandResponse>(new Error(404, $"Brand with id {id} not found"));

        var brandDto = _mapper.Map<ProductBrand, ProductBrandResponse>(brand);

        return Result.Success(brandDto);
    }

    public async Task<Result<IReadOnlyList<ProductBrandResponse>>> SearchBrandsAsync(string search)
    {
        var spec = new BaseSpecifications<ProductBrand>() { WhereCriteria = x => x.Name.Contains(search) };

        var brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync(spec);

        var brandsDto = _mapper.Map<IReadOnlyList<ProductBrand>, IReadOnlyList<ProductBrandResponse>>(brands);

        return Result.Success(brandsDto);
    }

    public async Task<Result<ProductBrandResponse>> CreateBrandAsync(ProductBrandRequest brandRequest)
    {
        var brand = _mapper.Map<ProductBrandRequest, ProductBrand>(brandRequest);

        await _unitOfWork.Repository<ProductBrand>().AddAsync(brand);

        var result = await _unitOfWork.CompleteAsync();

        if (result <= 0)
            return Result.Failure<ProductBrandResponse>(new Error(500, "Error occurred while saving brand"));

        var brandDto = _mapper.Map<ProductBrand, ProductBrandResponse>(brand);

        return Result.Success(brandDto);
    }

    public async Task<Result<ProductBrandResponse>> UpdateBrandAsync(int id, ProductBrandRequest brandRequest)
    {
        var brand = await _unitOfWork.Repository<ProductBrand>().GetEntityAsync(id);

        if (brand == null)
            return Result.Failure<ProductBrandResponse>(new Error(404, $"Brand with id {id} not found"));

        _mapper.Map(brandRequest, brand);

        var result = await _unitOfWork.CompleteAsync();

        if (result <= 0)
            return Result.Failure<ProductBrandResponse>(new Error(500, "Error occurred while updating brand"));

        var brandDto = _mapper.Map<ProductBrand, ProductBrandResponse>(brand);

        return Result.Success(brandDto);
    }

    public async Task<Result<ProductBrandResponse>> DeleteBrandAsync(int id)
    {
        var brand = await _unitOfWork.Repository<ProductBrand>().GetEntityAsync(id);

        if (brand == null)
            return Result.Failure<ProductBrandResponse>(new Error(404, $"Brand with id {id} not found"));

        _unitOfWork.Repository<ProductBrand>().Delete(brand);

        await _unitOfWork.CompleteAsync();

        var brandDto = _mapper.Map<ProductBrand, ProductBrandResponse>(brand);

        return Result.Success(brandDto);
    }

}