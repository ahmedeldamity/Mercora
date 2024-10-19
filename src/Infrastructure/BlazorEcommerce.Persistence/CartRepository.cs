using StackExchange.Redis;

namespace BlazorEcommerce.Persistence;
public class CartRepository(IConnectionMultiplexer connection) : IBasketRepository
{
    private readonly IDatabase _database = connection.GetDatabase();

    public async Task<Cart?> CreateOrUpdateBasketAsync(Cart cart)
    {
        var createdOrUpdated = await _database.StringSetAsync(cart.Id, JsonSerializer.Serialize(cart), TimeSpan.FromDays(30));

        if (createdOrUpdated is false) return null;

        return await GetBasketAsync(cart.Id);
    }
    public async Task<bool> DeleteBasketAsync(string basketId)
    {
        return await _database.KeyDeleteAsync(basketId);
    }
    public async Task<Cart?> GetBasketAsync(string basketId)
    {
        var basket = await _database.StringGetAsync(basketId);

        return basket.IsNullOrEmpty ? null : JsonSerializer.Deserialize<Cart>(basket!);
    }
}