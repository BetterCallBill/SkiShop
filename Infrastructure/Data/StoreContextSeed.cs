using System.Reflection;
using System.Text.Json;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data
{
    public class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext storeContext, ILoggerFactory loggerFactory)
        {
            try
            {
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                
                if (!storeContext.ProductBrands.Any())
                {
                    // Read JSON data file
                    var brandsData = File.ReadAllText("../Infrastructure/Data/SeedData/brands.json");

                    // Convert string to Objects
                    var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);

                    // Add to DB
                    foreach (var brand in brands)
                    {
                        storeContext.ProductBrands.Add(brand);
                    }

                    await storeContext.SaveChangesAsync();
                }

                if (!storeContext.ProductTypes.Any())
                {
                    // Read JSON data file
                    var typesData = File.ReadAllText("../Infrastructure/Data/SeedData/types.json");

                    // Convert string to Objects
                    var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);

                    // Add to DB
                    foreach (var type in types)
                    {
                        storeContext.ProductTypes.Add(type);
                    }

                    await storeContext.SaveChangesAsync();
                }

                if (!storeContext.Products.Any())
                {
                    // Read JSON data file
                    var productsData = File.ReadAllText("../Infrastructure/Data/SeedData/products.json");

                    // Convert string to Objects
                    var products = JsonSerializer.Deserialize<List<Product>>(productsData);

                    // Add to DB
                    foreach (var product in products)
                    {
                        storeContext.Products.Add(product);
                    }

                    await storeContext.SaveChangesAsync();
                }
                
                if (!storeContext.DeliveryMethods.Any())
                {
                    var dmData = File.ReadAllText(path + @"/Data/SeedData/delivery.json");

                    var methods = JsonSerializer.Deserialize<List<DeliveryMethod>>(dmData);

                    foreach (var item in methods)
                    {
                        storeContext.DeliveryMethods.Add(item);
                    }

                    await storeContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Create log
                var logger = loggerFactory.CreateLogger<StoreContextSeed>();
                logger.LogError(ex.Message);
            }
        }
    }
}