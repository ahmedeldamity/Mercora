using Core.Entities;
using Core.Entities.OrderEntities;
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

            if (_storeContext.Products.Count() == 0)
            {
                var ProductsJSONData = File.ReadAllText("../Repository/Store/DataSeeding/products.json");

                var products = JsonSerializer.Deserialize<List<Product>>(ProductsJSONData);

                if (products?.Count() > 0)
                {
                    foreach (var product in products)
                    {
                        _storeContext.Products.Add(product);
                    }
                }
            }


            if (_storeContext.OrderDeliveryMethods.Count() == 0)
            {
                var deliveryMethodsData = File.ReadAllText("../Repository/Store/DataSeeding/delivery.json");
                var deliveryMethods = JsonSerializer.Deserialize<List<OrderDeliveryMethod>>(deliveryMethodsData);

                if (deliveryMethods?.Count() > 0)
                {
                    foreach (var deliveryMethod in deliveryMethods)
                    {
                        _storeContext.OrderDeliveryMethods.Add(deliveryMethod);
                    }
                }
            }

            await _storeContext.SaveChangesAsync();
        }
    }
}