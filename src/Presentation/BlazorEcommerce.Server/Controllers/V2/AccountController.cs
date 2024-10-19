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
    public async Task<ActionResult<AppUserResponse>> SendEmailVerificationCode(RegisterVerificationRequest request)
    {
        var result = await accountService.SendEmailVerificationCode(request);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

}