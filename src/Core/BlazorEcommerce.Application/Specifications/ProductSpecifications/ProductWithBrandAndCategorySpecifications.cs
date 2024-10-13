using BlazorEcommerce.Domain.Entities.ProductEntities;

namespace BlazorEcommerce.Application.Specifications.ProductSpecifications;
public class ProductWithBrandAndCategorySpecifications : BaseSpecifications<Product>
{
    public ProductWithBrandAndCategorySpecifications(ProductSpecificationParameters specParams)
    {
        IncludesCriteria.Add(p => p.Brand);
        IncludesCriteria.Add(p => p.Category);

		WhereCriteria = p =>
			(string.IsNullOrEmpty(specParams.Search) ||
			 p.Name.ToLower().Contains(specParams.Search.ToLower()) ||
			 p.Description.ToLower().Contains(specParams.Search.ToLower())) &&
			(!specParams.BrandId.HasValue || p.BrandId == specParams.BrandId.Value) &&
			(!specParams.CategoryId.HasValue || p.CategoryId == specParams.CategoryId.Value);



		if (!string.IsNullOrEmpty(specParams.Sort))
        {
            switch (specParams.Sort)
            {
                case "name":
                    OrderBy = p => p.Name;
                    break;
                case "nameDesc":
                    OrderByDesc = p => p.Name;
                    break;
                case "price":
                    OrderBy = p => p.Price;
                    break;
                case "priceDesc":
                    OrderByDesc = p => p.Price;
                    break;
                default:
                    OrderBy = p => p.Price;
                    break;
            }
        }
        else
            OrderBy = p => p.Price;

        ApplyPagination((specParams.PageIndex - 1) * specParams.PageSize, specParams.PageSize);
    }

    public ProductWithBrandAndCategorySpecifications(int id)
    {
        WhereCriteria = p => p.Id == id;
        IncludesCriteria.Add(p => p.Brand);
        IncludesCriteria.Add(p => p.Category);
    }
}