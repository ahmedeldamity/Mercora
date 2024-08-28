namespace Service.ConfigurationData;
public class DatabaseConnections
{
    public string IdentityConnection { get; set; } = null!;
    public string StoreConnection { get; set; } = null!;
    public string RedisConnection { get; set; } = null!;
}