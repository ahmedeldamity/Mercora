using AutoMapper;
using Core.Dtos;
using Core.Entities;
using Core.ErrorHandling;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;

namespace Service;
public class BrandService(IUnitOfWork _unitOfWork, IMapper _mapper): IBrandService
{
    public async Task<Result<IReadOnlyList<ProductBrandResponse>>> GetBrandsAsync()
    {
        var brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();

        var brandsDto = _mapper.Map<IReadOnlyList<ProductBrand>, IReadOnlyList<ProductBrandResponse>>(brands);

        return Result.Success(brandsDto);
    }
}