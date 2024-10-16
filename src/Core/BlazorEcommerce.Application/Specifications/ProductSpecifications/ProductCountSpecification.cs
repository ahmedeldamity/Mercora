using BlazorEcommerce.Domain.Entities.ProductEntities;

namespace BlazorEcommerce.Application.Specifications.ProductSpecifications;
public class ProductCountSpecification : BaseSpecifications<Product>
{
    public ProductCountSpecification(ProductSpecificationParameters specParams)
    {
        WhereCriteria = p =>
	        (string.IsNullOrEmpty(specParams.Search) ||
	         p.Name.ToLower().Contains(specParams.Search.ToLower()) ||
	         p.Description.ToLower().Contains(specParams.Search.ToLower())) &&
	        (specParams.BrandId == null || specParams.BrandId == 0 || p.BrandId == specParams.BrandId.Value) &&
	        (specParams.CategoryId == null || specParams.CategoryId == 0 || p.CategoryId == specParams.CategoryId.Value);
	}
}