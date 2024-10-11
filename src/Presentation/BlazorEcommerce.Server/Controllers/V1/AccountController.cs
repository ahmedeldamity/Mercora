using Asp.Versioning;
using BlazorEcommerce.Application.Dtos;
using BlazorEcommerce.Application.Interfaces.Services;
using BlazorEcommerce.Server.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BlazorEcommerce.Server.Controllers.V1;
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[EnableRateLimiting("SlidingWindowPolicy")]
public class AccountController(IAccountService accountService) : ControllerBase
{
	[HttpPost("register-email-verification-code")]
	[EnableRateLimiting("ConcurrencyPolicy")]
	public async Task<ActionResult<AppUserResponse>> SendRegisterEmailVerificationCode(string email)
	{
		var result = await accountService.SendEmailVerificationCode(email);

		return result.IsSuccess ? Ok() : result.ToProblem();
	}
	
	[HttpPost("login-email-verification-code")]
	[EnableRateLimiting("ConcurrencyPolicy")]
	public async Task<ActionResult<AppUserResponse>> SendLoginEmailVerificationCode(string email)
	{
			var result = await accountService.SendEmailVerificationCode(email, 1, false);

		return result.IsSuccess ? Ok() : result.ToProblem();
	}

	[HttpPost("register")]
	[EnableRateLimiting("ConcurrencyPolicy")]
	public async Task<ActionResult> Register(RegisterCodeVerificationRequest model)
	{
		var result = await accountService.VerifyCodeForRegister(model);

		return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
	}

	[HttpPost("login")]
	[EnableRateLimiting("ConcurrencyPolicy")]
	public async Task<ActionResult> Login(LoginCodeVerificationRequest model)
	{
		var result = await accountService.VerifyCodeForLogin(model);

		return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
	}
	
    [HttpGet("get-current-user")]
    [Authorize]
    public async Task<ActionResult<AppUserResponse>> GetCurrentUser()
    {
        var result = await accountService.GetCurrentUser(User);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [Authorize]
    [HttpGet("get-current-user-address")]
    public async Task<ActionResult<UserAddressResponse>> GetCurrentUserAddress()
    {
        var result = await accountService.GetCurrentUserAddress(User);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [Authorize]
    [HttpPut("update-current-user-address")]
    public async Task<ActionResult<UserAddressResponse>> UpdateUserAddress(UserAddressResponse updatedAddress)
    {
        var result = await accountService.UpdateUserAddress(updatedAddress, User);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

	[HttpGet("google-login")]
	public ActionResult GoogleLogin()
	{
		var result = accountService.GoogleLogin();

		return Redirect(result);
	}

	[HttpGet("google-response")]
	public async Task<ActionResult> GoogleResponse(string code)
	{
		var result = await accountService.GoogleResponse(code);

		return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
	}

	[HttpGet("github-login")]
	public ActionResult GithubLogin()
	{
		var result = accountService.GithubLogin();

		return Redirect(result);
	}

	[HttpGet("github-response")]
	public async Task<ActionResult> GithubResponse(string code)
	{
		var result = await accountService.GithubResponse(code);

		return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
	}

	[HttpGet("refresh-token")]
    public async Task<ActionResult> CreateAccessTokenByRefreshToken()
    {
        var result = await accountService.CreateAccessTokenByRefreshTokenAsync();

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("revoke-token")]
    public async Task<ActionResult> RevokeRefreshToken()
    {
        var result = await accountService.RevokeRefreshTokenAsync();

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}