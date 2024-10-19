namespace BlazorEcommerce.Domain.Entities.IdentityEntities;

[Owned]
public class RefreshToken
{
    public string Token { get; set; } = null!;
    public DateTime ExpireAt { get; set; }
    public bool IsExpired => DateTime.Now >= ExpireAt;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? RevokedAt { get; set; }
    public bool IsActive => RevokedAt == null && !IsExpired;
}