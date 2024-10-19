namespace BlazorEcommerce.Server.Controllers.V1;
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[EnableRateLimiting("SlidingWindowPolicy")]
public class AccountController(IAccountService accountService) : ControllerBase
{
	[HttpPost("send-email-verification-code")]
	[EnableRateLimiting("ConcurrencyPolicy")]
	public async Task<ActionResult<AppUserResponse>> SendEmailVerificationCode(RegisterVerificationRequest request)
	{
		var result = await accountService.SendEmailVerificationCode(request);

		return result.IsSuccess ? Ok() : result.ToProblem();
	}
    
    [HttpPost("register")]
	[EnableRateLimiting("ConcurrencyPolicy")]
	public async Task<ActionResult> Register(RegisterRequest registerRequest)
	{
		var result = await accountService.VerifyCodeForRegister(registerRequest);

		return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
	}

    [HttpPost("login")]
    [EnableRateLimiting("ConcurrencyPolicy")]
    public async Task<ActionResult> Login(LoginRequest model)
    {
        var result = await accountService.Login(model);

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

		return result.IsSuccess ?
            Redirect($"https://localhost:7055/check-authentication?auth=success&email={result.Value}") :
            Redirect("https://localhost:7055/check-authentication?auth=failure");
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

        return result.IsSuccess ?
            Redirect($"https://localhost:7055/check-authentication?auth=success&email={result.Value}") :
            Redirect("https://localhost:7055/check-authentication?auth=failure");
    }
	
	[HttpGet("get-user")]
	public async Task<ActionResult> GetUser(string email)
	{
		var result = await accountService.GetUserAsync(email);

		return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
	}

	[HttpPost("send-reset-password-code")]
    [EnableRateLimiting("ConcurrencyPolicy")]
    public async Task<ActionResult<AppUserResponse>> SendResetPasswordCode(ResetPasswordRequest request)
    {
        var result = await accountService.SendResetPasswordCode(request);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting("ConcurrencyPolicy")]
    public async Task<ActionResult<AppUserResponse>> ResetPassword(ResetPassword resetUserData)
    {
        var result = await accountService.ResetPassword(resetUserData);

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