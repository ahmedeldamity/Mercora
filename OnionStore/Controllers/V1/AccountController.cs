using API.Extensions;
using Asp.Versioning;
using Core.Dtos;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[EnableRateLimiting("SlidingWindowPolicy")]
public class AccountController(IAccountService _accountService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AppUserResponse>> Register(RegisterRequest model)
    {
        var result = await _accountService.Register(model);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("login")]
    public async Task<ActionResult<AppUserResponse>> Login(LoginRequest model)
    {
        var result = await _accountService.Login(model);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<AppUserResponse>> GetCurrentUser()
    {
        var result = await _accountService.GetCurrentUser(User);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [Authorize]
    [HttpGet("address")]
    public async Task<ActionResult<UserAddressResponse>> GetCurrentUserAddress()
    {
        var result = await _accountService.GetCurrentUserAddress(User);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [Authorize]
    [HttpPut("address")]
    public async Task<ActionResult<UserAddressResponse>> UpdateUserAddress(UserAddressResponse updatedAddress)
    {
        var result = await _accountService.UpdateUserAddress(updatedAddress, User);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("google-login")]
    public async Task<ActionResult<AppUserResponse>> GoogleLogin([FromBody] string credential)
    {
        var result = await _accountService.GoogleLogin(credential);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("refresh-token")]
    public async Task<IActionResult> CreateAccessTokenByRefreshToken()
    {
        var result = await _accountService.CreateAccessTokenByRefreshTokenAsync();

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeRefreshToken()
    {
        var result = await _accountService.RevokeRefreshTokenAsync();

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}