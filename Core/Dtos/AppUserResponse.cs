﻿namespace Core.Dtos;

public class AppUserResponse
{
    public string DisplayName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
}