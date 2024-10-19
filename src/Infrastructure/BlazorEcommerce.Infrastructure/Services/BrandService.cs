namespace BlazorEcommerce.Infrastructure.Services;
public class BrandService(IUnitOfWork unitOfWork, IMapper mapper) : IBrandService
{
    public async Task<Result<IReadOnlyList<ProductBrandResponse>>> GetBrandsAsync()
    {
        var brands = await unitOfWork.Repository<ProductBrand>().GetAllAsync();

        var brandsDto = mapper.Map<IReadOnlyList<ProductBrand>, IReadOnlyList<ProductBrandResponse>>(brands);

        return Result.Success(brandsDto);
    }

    public async Task<Result<ProductBrandResponse>> GetBrandByIdAsync(int id)
    {
        var brand = await unitOfWork.Repository<ProductBrand>().GetEntityAsync(id);

        if (brand == null)
            return Result.Failure<ProductBrandResponse>(new Error(404, $"Brand with id {id} not found"));

        var brandDto = mapper.Map<ProductBrand, ProductBrandResponse>(brand);

        return Result.Success(brandDto);
    }

    public async Task<Result<IReadOnlyList<ProductBrandResponse>>> SearchBrandsAsync(string search)
    {
        var spec = new BaseSpecifications<ProductBrand> { WhereCriteria = x => x.Name.Contains(search) };

        var brands = await unitOfWork.Repository<ProductBrand>().GetAllAsync(spec);

        var brandsDto = mapper.Map<IReadOnlyList<ProductBrand>, IReadOnlyList<ProductBrandResponse>>(brands);

        return Result.Success(brandsDto);
    }

    public async Task<Result<ProductBrandResponse>> CreateBrandAsync(ProductBrandRequest brandRequest)
    {
        var brand = mapper.Map<ProductBrandRequest, ProductBrand>(brandRequest);

        await unitOfWork.Repository<ProductBrand>().AddAsync(brand);

        var result = await unitOfWork.CompleteAsync();

        if (result <= 0)
            return Result.Failure<ProductBrandResponse>(new Error(500, "Error occurred while saving brand"));

        var brandDto = mapper.Map<ProductBrand, ProductBrandResponse>(brand);

        return Result.Success(brandDto);
    }

    public async Task<Result<ProductBrandResponse>> UpdateBrandAsync(int id, ProductBrandRequest brandRequest)
    {
        var brand = await unitOfWork.Repository<ProductBrand>().GetEntityAsync(id);

        if (brand == null)
            return Result.Failure<ProductBrandResponse>(new Error(404, $"Brand with id {id} not found"));

        mapper.Map(brandRequest, brand);

        unitOfWork.Repository<ProductBrand>().Update(brand);

        var result = await unitOfWork.CompleteAsync();

        if (result <= 0)
            return Result.Failure<ProductBrandResponse>(new Error(500, "Error occurred while updating brand"));

        var brandDto = mapper.Map<ProductBrand, ProductBrandResponse>(brand);

        return Result.Success(brandDto);
    }

    public async Task<Result<ProductBrandResponse>> DeleteBrandAsync(int id)
    {
        var brand = await unitOfWork.Repository<ProductBrand>().GetEntityAsync(id);

        if (brand == null)
            return Result.Failure<ProductBrandResponse>(new Error(404, $"Brand with id {id} not found"));

        unitOfWork.Repository<ProductBrand>().Delete(brand);

        await unitOfWork.CompleteAsync();

        var brandDto = mapper.Map<ProductBrand, ProductBrandResponse>(brand);

        return Result.Success(brandDto);
    }

}