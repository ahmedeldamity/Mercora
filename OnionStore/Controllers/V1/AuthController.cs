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

public class AuthController(IAuthService authService) : ControllerBase
{
    [Authorize]
    [HttpPost("send-email-verification-code")]
    [EnableRateLimiting("ConcurrencyPolicy")]
    public async Task<ActionResult<AppUserResponse>> SendEmailVerificationCode()
    {
        var result = await authService.SendEmailVerificationCode(User);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [Authorize]
    [HttpPost("verify-register-code")]
    [EnableRateLimiting("ConcurrencyPolicy")]
    public async Task<ActionResult> VerifyRegisterCode(CodeVerificationRequest model)
    {
        var result = await authService.VerifyRegisterCode(model, User);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("send-password-verification-code")]
    public async Task<ActionResult> SendPasswordResetEmail(EmailRequest email)
    {
        var result = await authService.SendPasswordResetEmail(email);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("Verify-Reset-Code")]
    public async Task<ActionResult> VerifyResetCode(CodeVerificationRequest model)
    {
        var result = await authService.VerifyResetCode(model, User);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<ActionResult> ChangePassword(ChangePasswordRequest model)
    {
        var result = await authService.ChangePassword(model, User);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

}