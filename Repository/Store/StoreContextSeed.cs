using Core.Entities;
using Core.Entities.Product_Entities;
using Repository.Store;
using System.Text.Json;

namespace Repository.Data
{
    public class StoreContextSeed
    {
        public async static Task SeedProductDataAsync(StoreContext _storeContext)
        {
            if (_storeContext.Brands.Count() == 0)
            {
                var brandsJSONData = File.ReadAllText("../Repository/Store/DataSeeding/brands.json");

                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsJSONData);

                if (brands?.Count() > 0)
                {
                    foreach (var brand in brands)
                    {
                        _storeContext.Brands.Add(brand);
                    }
                }
            }

            if (_storeContext.Categories.Count() == 0)
            {
                var catrgoriesJSONData = File.ReadAllText("../Repository/Store/DataSeeding/categories.json");

                var categories = JsonSerializer.Deserialize<List<ProductCategory>>(catrgoriesJSONData);

                if (categories?.Count() > 0)
                {
                    foreach (var category in categories)
                    {
                        _storeContext.Categories.Add(category);
                    }
                }
            }

            await _storeContext.SaveChangesAsync();
        }
    }
}