using API.Extensions;
using Asp.Versioning;
using Core.Dtos;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V2;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("2.0")]
public class AuthController(IAuthService _authService) : ControllerBase
{
    [Authorize]
    [HttpPost("send-email-verification-code")]
    [MapToApiVersion("2.0")]
    public async Task<ActionResult<AppUserResponse>> SendEmailVerificationCodeV2()
    {
        var result = await _authService.SendEmailVerificationCodeV2(User);

        return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
    }
}