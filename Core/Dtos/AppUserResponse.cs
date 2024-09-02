using System.Text.Json.Serialization;

namespace Core.Dtos;
public record AppUserResponse
{
    public string DisplayName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;

    [JsonIgnore]
    public string? RefreshToken { get; set; }

    public DateTime RefreshTokenExpireAt { get; set; }
}

public record AppUserResponseV20
{
    public string DisplayName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    [JsonIgnore]
    public string? RefreshToken { get; set; }

    public string RefreshTokenExpireAt { get; set; } = null!;
}

public record AppUserResponseV21
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    [JsonIgnore]
    public string? RefreshToken { get; set; }

    public string RefreshTokenExpireAt { get; set; } = null!;
}