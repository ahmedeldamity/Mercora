using BlazorEcommerce.Domain.Entities.ProductEntities;

namespace BlazorEcommerce.Application.Specifications.ProductSpecifications;
public class ProductCountSpecification : BaseSpecifications<Product>
{
    public ProductCountSpecification(ProductSpecificationParameters specParams)
    {
        WhereCriteria =
           p => (string.IsNullOrEmpty(specParams.Search) || p.Name.ToLower().Contains(specParams.Search.ToLower())) &&
                (!specParams.BrandId.HasValue || p.BrandId == specParams.BrandId.Value) &&
                (!specParams.CategoryId.HasValue || p.CategoryId == specParams.CategoryId.Value);
    }
}