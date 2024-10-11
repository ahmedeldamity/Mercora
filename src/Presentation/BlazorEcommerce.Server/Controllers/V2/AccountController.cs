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
	[HttpPost("send-email-verification-code")]
	[EnableRateLimiting("ConcurrencyPolicy")]
	public async Task<ActionResult<AppUserResponse>> SendEmailVerificationCodeV2(string email)
	{
		var result = await accountService.SendEmailVerificationCodeV2(email);

		return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
	}
}