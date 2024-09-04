using Core.Entities;
using Core.Entities.OrderEntities;
using Core.Entities.Product_Entities;
using System.Text.Json;

namespace Repository.Store;
public class StoreContextSeed
{
    public async static Task SeedProductDataAsync(StoreContext _storeContext)
    {
        if (!_storeContext.Brands.Any())
        {
            var brandsFilePath = Path.Combine("DataSeeding", "brands.json");

            var brandsJSONData = await File.ReadAllTextAsync(brandsFilePath);

            var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsJSONData);

            if (brands!.Count > 0)
            {
                foreach (var brand in brands)
                {
                    _storeContext.Brands.Add(brand);
                }
            }
        }

        if (!_storeContext.Categories.Any())
        {
            var categoriesFilePath = Path.Combine("DataSeeding", "categories.json");

            var catrgoriesJSONData = await File.ReadAllTextAsync(categoriesFilePath);

            var categories = JsonSerializer.Deserialize<List<ProductCategory>>(catrgoriesJSONData);

            if (categories!.Count > 0)
            {
                foreach (var category in categories)
                {
                    _storeContext.Categories.Add(category);
                }
            }
        }

        if (!_storeContext.Products.Any())
        {
            var productsFilePath = Path.Combine("DataSeeding", "products.json");

            var ProductsJSONData = await File.ReadAllTextAsync(productsFilePath);

            var products = JsonSerializer.Deserialize<List<Product>>(ProductsJSONData);

            if (products!.Count > 0)
            {
                foreach (var product in products)
                {
                    _storeContext.Products.Add(product);
                }
            }
        }

        if (!_storeContext.OrderDeliveryMethods.Any())
        {
            var deliveryMethodsFilePath = Path.Combine("DataSeeding", "delivery.json");

            var deliveryMethodsData = await File.ReadAllTextAsync(deliveryMethodsFilePath);

            var deliveryMethods = JsonSerializer.Deserialize<List<OrderDeliveryMethod>>(deliveryMethodsData);

            if (deliveryMethods!.Count > 0)
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