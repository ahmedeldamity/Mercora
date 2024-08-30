namespace Core.Dtos;
public record AppUserResponse
{
    public string DisplayName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
}