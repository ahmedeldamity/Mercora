using Core.Entities.Product_Entities;

namespace Core.Specifications.ProductSpecifications;
public class ProductCountSpecification : BaseSpecifications<Product>
{
    public ProductCountSpecification(ProductSpecificationParameters specParams)
    {
        WhereCriteria =
           p => (string.IsNullOrEmpty(specParams.search) || p.Name.ToLower().Contains(specParams.search.ToLower())) &&
           (!specParams.brandId.HasValue || p.BrandId == specParams.brandId.Value) &&
           (!specParams.categoryId.HasValue || p.CategoryId == specParams.categoryId.Value);
    }
}