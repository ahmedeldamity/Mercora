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

public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("send-email-verification-code")]
    [EnableRateLimiting("ConcurrencyPolicy")]
    public async Task<ActionResult<AppUserResponse>> SendEmailVerificationCode(string email)
    {
        var result = await authService.SendEmailVerificationCode(email);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("verify-register-code")]
    [EnableRateLimiting("ConcurrencyPolicy")]
    public async Task<ActionResult> VerifyRegisterCode(CodeVerificationRequest model)
    {
        var result = await authService.VerifyRegisterCode(model);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}