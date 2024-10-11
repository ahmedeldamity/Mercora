using Asp.Versioning;
using BlazorEcommerce.Application.Dtos;
using BlazorEcommerce.Application.Interfaces.Services;
using BlazorEcommerce.Server.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BlazorEcommerce.Server.Controllers.V2;
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("2.0")]
[ApiVersion("2.1")]
[EnableRateLimiting("FixedWindowPolicy")]
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
		var result = await accountService.SendEmailVerificationCode(email, 2,false);

		return result.IsSuccess ? Ok() : result.ToProblem();
	}

}