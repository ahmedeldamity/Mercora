namespace BlazorEcommerce.Server.Controllers.V1;
public class BrandController(IBrandService brandService) : BaseController
{
    [HttpGet]
    [Cached(600)]
    public async Task<ActionResult> GetBrands()
    {
        var result = await brandService.GetBrandsAsync();

        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetBrandById(int id)
    {
        var result = await brandService.GetBrandByIdAsync(id);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("search")]
    public async Task<ActionResult> SearchBrands(string search)
    {
        var result = await brandService.SearchBrandsAsync(search);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult> CreateBrand(ProductBrandRequest brandRequest)
    {
        var result = await brandService.CreateBrandAsync(brandRequest);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateBrand(int id, ProductBrandRequest brandRequest)
    {
        var result = await brandService.UpdateBrandAsync(id, brandRequest);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteBrand(int id)
    {
        var result = await brandService.DeleteBrandAsync(id);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

}