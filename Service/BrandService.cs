using AutoMapper;
using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Shared.Dtos;
using Shared.Helpers;

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