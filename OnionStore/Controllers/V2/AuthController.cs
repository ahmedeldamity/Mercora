using API.Extensions;
using Asp.Versioning;
using Core.Dtos;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers.V2;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("2.0")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [Authorize]
    [HttpPost("send-email-verification-code")]
    [EnableRateLimiting("ConcurrencyPolicy")]
    public async Task<ActionResult<AppUserResponse>> SendEmailVerificationCodeV2()
    {
        var result = await authService.SendEmailVerificationCodeV2(User);

        return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
    }

    [HttpPost("send-password-verification-code")]
    public async Task<ActionResult> SendPasswordResetEmailV2(EmailRequest email)
    {
        var result = await authService.SendPasswordResetEmailV2(email);

        return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
    }

}