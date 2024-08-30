using System.Text.Json.Serialization;

namespace Core.Dtos;
public record AppUserResponse
{
    public string DisplayName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;

    [JsonIgnore]
    public string? RefreshToken { get; set; }

    public DateOnly RefreshTokenExpireAt { get; set; }
}