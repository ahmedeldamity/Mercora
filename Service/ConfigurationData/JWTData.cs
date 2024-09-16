namespace Service.ConfigurationData;
public class JwtData
{
    public string SecretKey { get; set; } = null!;
    public string ValidAudience { get; set; } = null!;
    public string ValidIssuer { get; set; } = null!;
    public double DurationInMinutes { get; set; }
    public double RefreshTokenExpirationInDays { get; set; }
}