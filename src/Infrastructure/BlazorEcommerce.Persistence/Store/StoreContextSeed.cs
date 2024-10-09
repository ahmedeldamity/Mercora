using BlazorEcommerce.Domain.Entities.OrderEntities;
using BlazorEcommerce.Domain.Entities.ProductEntities;
using System.Text.Json;

namespace BlazorEcommerce.Persistence.Store;
public class StoreContextSeed
{
    public static async Task SeedProductDataAsync(StoreContext storeContext)
    {
        if (!storeContext.Brands.Any())
        {
            //var brandsFilePath = Path.Combine("DataSeeding", "brands.json");

            var brandsFilePath = Path.Combine(AppContext.BaseDirectory, "../../../../../Infrastructure/BlazorEcommerce.Persistence/Store/DataSeeding/brands.json");

			var brandsJsonData = await File.ReadAllTextAsync(brandsFilePath);

            var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsJsonData);

            if (brands?.Count > 0) 
            {
                foreach (var brand in brands)
                {
                    storeContext.Brands.Add(brand);
                }
            }
        }

        if (!storeContext.Categories.Any())
        {
            //var categoriesFilePath = Path.Combine("DataSeeding", "categories.json");

            var categoriesFilePath = Path.Combine(AppContext.BaseDirectory, "../../../../../Infrastructure/BlazorEcommerce.Persistence/Store/DataSeeding/categories.json");

			var categoriesJsonData = await File.ReadAllTextAsync(categoriesFilePath);

            var categories = JsonSerializer.Deserialize<List<ProductCategory>>(categoriesJsonData);

            if (categories?.Count > 0)
            {
                foreach (var category in categories)
                {
                    storeContext.Categories.Add(category);
                }
            }
        }

        if (!storeContext.Products.Any())
        {
			//var productsFilePath = Path.Combine("DataSeeding", "products.json");

			var productsFilePath = Path.Combine(AppContext.BaseDirectory, "../../../../../Infrastructure/BlazorEcommerce.Persistence/Store/DataSeeding/products.json");

			var productsJsonData = await File.ReadAllTextAsync(productsFilePath);

            var products = JsonSerializer.Deserialize<List<Product>>(productsJsonData);

            if (products?.Count > 0)
            {
                foreach (var product in products)
                {
                    storeContext.Products.Add(product);
                }
            }
        }

        if (!storeContext.OrderDeliveryMethods.Any())
        {
            //var deliveryMethodsFilePath = Path.Combine("DataSeeding", "delivery.json");

            var deliveryMethodsFilePath = Path.Combine(AppContext.BaseDirectory, "../../../../../Infrastructure/BlazorEcommerce.Persistence/Store/DataSeeding/delivery.json");

			var deliveryMethodsData = await File.ReadAllTextAsync(deliveryMethodsFilePath);

            var deliveryMethods = JsonSerializer.Deserialize<List<OrderDeliveryMethod>>(deliveryMethodsData);

            if (deliveryMethods?.Count > 0)
            {
                foreach (var deliveryMethod in deliveryMethods)
                {
                    storeContext.OrderDeliveryMethods.Add(deliveryMethod);

                }
            }
        }

        await storeContext.SaveChangesAsync();
    }
}