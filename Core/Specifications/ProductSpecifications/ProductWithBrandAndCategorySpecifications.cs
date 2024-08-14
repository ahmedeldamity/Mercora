using Core.Entities.Product_Entities;

namespace Core.Specifications.ProductSpecifications
{
    public class ProductWithBrandAndCategorySpecifications : BaseSpecifications<Product>
    {
        public ProductWithBrandAndCategorySpecifications(ProductSpecificationParameters specParams)
        {
            IncludesCriteria.Add(p => p.Brand);
            IncludesCriteria.Add(p => p.Category);

            WhereCriteria =
               p => (string.IsNullOrEmpty(specParams.search) || p.Name.ToLower().Contains(specParams.search.ToLower())) &&
               (!specParams.brandId.HasValue || p.BrandId == specParams.brandId.Value) &&
               (!specParams.categoryId.HasValue || p.CategoryId == specParams.categoryId.Value);

            if (!string.IsNullOrEmpty(specParams.sort))
            {
                switch (specParams.sort)
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

        }

        public ProductWithBrandAndCategorySpecifications(int id)
        {
            WhereCriteria = p => p.Id == id;
            IncludesCriteria.Add(p => p.Brand);
            IncludesCriteria.Add(p => p.Category);
        }
    }
}