using BlazorEcommerce.Domain.Entities.ProductEntities;

namespace BlazorEcommerce.Application.Specifications.ProductSpecifications;
public class ProductFeaturedSpecifications: BaseSpecifications<Product>
{
	public ProductFeaturedSpecifications()
	{
		IncludesCriteria.Add(p => p.Brand);
		IncludesCriteria.Add(p => p.Category);

		WhereCriteria = p => p.Featured;
	}
}