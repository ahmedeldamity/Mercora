using Newtonsoft.Json;
using StackExchange.Redis;

namespace BlazorEcommerce.Infrastructure.Services;
public class AccountService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IHttpContextAccessor httpContextAccessor,
IOptions<Urls> urls, IOptions<JwtData> jWtData, IOptions<GoogleData> googleData, IOptions<GithubData> githubData,
IEmailSettingService emailSettings, IConnectionMultiplexer connection, IMapper mapper) : IAccountService
{
    private readonly JwtData _jWtData = jWtData.Value;
    private readonly GoogleData _googleData = googleData.Value;
    private readonly GithubData _githubData = githubData.Value;
    private readonly Urls _urls = urls.Value;
    private readonly IDatabase _database = connection.GetDatabase();

	public async Task<Result> SendEmailVerificationCode(RegisterVerificationRequest registerRequest)
	{
		var user = await userManager.FindByEmailAsync(registerRequest.Email);

		if (user is not null)
			return Result.Failure<AppUserResponse>(new Error(400, "The email address you entered is already taken, Please try a different one."));

		var code = AccountServiceHelper.GenerateSecureCode();

        var securedCode = AccountServiceHelper.HashCode(code);

        var subject = $"✅ {registerRequest.Email.Split('@')[0]}, Please confirm your email address";

		var redirectUrl = $"https://localhost:7055/verify-email/{registerRequest.Email}/{securedCode}";

		var body = AccountServiceHelper.RegisterEmailBody(redirectUrl, registerRequest.DisplayName);

        var key = $"verification:{registerRequest.Email}";
        
		var model = new RegisterVerificationData(registerRequest.Email, securedCode, registerRequest.DisplayName);

        await _database.StringSetAsync(key, JsonConvert.SerializeObject(model), TimeSpan.FromMinutes(5));
        
        var emailToSend = new EmailResponse(subject, body, registerRequest.Email);

		BackgroundJob.Enqueue(() => emailSettings.SendEmailMessage(emailToSend));

		return Result.Success();
	}
    
    public async Task<Result<AppUserResponse>> VerifyCodeForRegister(RegisterRequest registerRequest)
	{
		var key = $"verification:{registerRequest.Email}";

		var response = await _database.StringGetAsync(key);

		_database.KeyDelete(key);

		if (response.IsNullOrEmpty)
			return Result.Failure<AppUserResponse>(new Error(400, "The reset code is missing or invalid. Please request a new reset code."));

		var model = JsonConvert.DeserializeObject<RegisterVerificationData>(response!);

		if(model is null)
			return Result.Failure<AppUserResponse>(new Error(400, "The reset code is missing or invalid. Please request a new reset code."));

		var user = await userManager.FindByEmailAsync(model.Email);

		if (user is not null)
			return Result.Failure<AppUserResponse>(new Error(400, "The email address you entered is already taken, Please try a different one."));

		if (!AccountServiceHelper.ConstantComparison(registerRequest.Code, model.Code))
			return Result.Failure<AppUserResponse>(new Error(400, "The reset code is missing or invalid. Please request a new reset code."));

		var tryRegister = await Register(model.Name, model.Email);
		
		if (tryRegister.IsSuccess is false)
			return Result.Failure<AppUserResponse>(new Error(400, "Failed to create user."));

		var userResponse = await GetUserAsync(model.Email);

		return userResponse;
	}

	public async Task<Result<AppUserResponse>> GetUserAsync(string email)
	{
		var user = await userManager.FindByEmailAsync(email);

		if (user is null)
			return Result.Failure<AppUserResponse>(new Error(404, "The user is not available in our system."));

		var token = GenerateAccessTokenAsync(user.Id, user.DisplayName, email);

		RefreshToken refreshToken;

		if (user.RefreshTokens is not null && user.RefreshTokens.Any(t => t.IsActive))
			refreshToken = user.RefreshTokens.First(t => t.IsActive);
		else
		{
			refreshToken = GenerateRefreshToken();
			user.RefreshTokens!.Add(refreshToken);
			await userManager.UpdateAsync(user);
		}
		
		SetRefreshTokenInCookie(refreshToken.Token, refreshToken.ExpireAt);

		var userResponse = new AppUserResponse(user.DisplayName, email, token, refreshToken.ExpireAt);

		return Result.Success(userResponse);
	}

    public async Task<Result<AppUserResponse>> Login(LoginRequest model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);

        if (user is null || model.Password is null)
            return Result.Failure<AppUserResponse>(new Error(400, "The email or password you entered is incorrect, Check your credentials and try again."));

        var hasPassword = await userManager.HasPasswordAsync(user);

        if (hasPassword is false)
            return Result.Failure<AppUserResponse>(new Error(400, "You need to reset your password."));

        var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);

        if (result.Succeeded is false)
        {
            var errors = string.Join(", ", "The email or password you entered is incorrect, Check your credentials and try again.");
            return Result.Failure<AppUserResponse>(new Error(400, errors));
        }

        var userResponse = await GetUserAsync(model.Email);

        return userResponse;
    }

    public string GoogleLogin()
    {
	    var googleOAuthUrl = $"https://accounts.google.com/o/oauth2/v2/auth/oauthchooseaccount?" +
	                         $"client_id={_googleData.ClientId}" +
	                         $"&redirect_uri={_urls.BaseUrl}/api/v1/Account/google-response" +
	                         $"&response_type=code" +
	                         $"&scope=openid%20profile%20email";

		return googleOAuthUrl;
    }

    public async Task<Result<string>> GoogleResponse(string code)
    {
	    if (string.IsNullOrEmpty(code))
		    return Result.Failure<string>(new Error(400, "Authorization code is missing."));

	    var tokenResponse = await AccountServiceHelper.GetGoogleAccessTokenAsync(code, _googleData.ClientId, _googleData.ClientSecret, _urls.BaseUrl);

	    if (tokenResponse == null)
		    return Result.Failure<string>(new Error(400, "Failed to get access token from Google."));

	    var googleUserInfo = await AccountServiceHelper.GetGoogleUserInfoAsync(tokenResponse.Access_Token);

	    if (googleUserInfo == null)
		    return Result.Failure<string>(new Error(400, "Failed to get user information from Google."));

	    var user = await userManager.FindByEmailAsync(googleUserInfo.Email);

	    if (user == null)
	    {
		    var response = await Register(googleUserInfo.Name, googleUserInfo.Email);

			if (response.IsSuccess is false)
                return Result.Failure<string>(new Error(400, "Failed to create user."));

			return Result.Success<string>(googleUserInfo.Email);
	    }

        return Result.Success<string>(googleUserInfo.Email);
    }

	public string GithubLogin()
	{
		var githubOAuthUrl = $"https://github.com/login/oauth/authorize?" +
							  $"client_id={_githubData.ClientId}" +
							  $"&redirect_uri={_urls.BaseUrl}/api/v1/Account/github-response" +
							  $"&scope=user:email";

		return githubOAuthUrl;
	}

	public async Task<Result<string>> GithubResponse(string code)
	{
		if (string.IsNullOrEmpty(code))
			return Result.Failure<string>(new Error(400, "Authorization code is missing."));

		var tokenResponse = await AccountServiceHelper.GetGitHubAccessTokenAsync(code, _githubData.ClientId, _githubData.ClientSecret);

		if (tokenResponse == null)
			return Result.Failure<string>(new Error(400, "Failed to get access token from GitHub."));

		var githubUserInfo = await AccountServiceHelper.GetGitHubUserInfoAsync(tokenResponse.Access_Token);

		if (githubUserInfo == null)
			return Result.Failure<string>(new Error(400, "Failed to get user information from GitHub."));

		var user = await userManager.FindByEmailAsync(githubUserInfo.Email);

		if (user == null)
		{
            var response = await Register(githubUserInfo.Name, githubUserInfo.Email);

            if (response.IsSuccess is false)
                return Result.Failure<string>(new Error(400, "Failed to create user."));

            return Result.Success<string>(githubUserInfo.Email);
		}

		return Result.Success<string>(githubUserInfo.Email);
	}

    public async Task<Result> SendResetPasswordCode(ResetPasswordRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return Result.Failure<AppUserResponse>(new Error(400, "The email address you entered is not registered in our system."));

        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        var encodedToken = System.Net.WebUtility.UrlEncode(token);

        var subject = $"✅ {request.Email.Split('@')[0]}, Reset your password for Mercora";

        var redirectUrl = $"https://localhost:7055/resetpassword/change/{user.Email}/{encodedToken}";

        var body = AccountServiceHelper.ResetPasswordEmailBody(redirectUrl, request.Email.Split('@')[0]);

        var key = $"reset:{request.Email}";

		var model = new ResetPasswordData(request.Email);

        await _database.StringSetAsync(key, JsonConvert.SerializeObject(model), TimeSpan.FromMinutes(5));

        var emailToSend = new EmailResponse(subject, body, request.Email);

        BackgroundJob.Enqueue(() => emailSettings.SendEmailMessage(emailToSend));

        return Result.Success();
    }

    public async Task<Result<AppUserResponse>> ResetPassword(ResetPassword resetUserData)
    {
        var user = await userManager.FindByEmailAsync(resetUserData.Email);

        if (user is null)
            return Result.Failure<AppUserResponse>(new Error(400, "The email address you entered is not registered in our system."));

        var response = await _database.StringGetAsync($"reset:{resetUserData.Email}");

        if (response.IsNullOrEmpty)
            return Result.Failure<AppUserResponse>(new Error(400, "The reset code is missing or invalid. Please request a new reset code."));

        var model = JsonConvert.DeserializeObject<ResetPasswordData>(response!);

        if (model is null)
            return Result.Failure<AppUserResponse>(new Error(400, "The reset code is missing or invalid. Please request a new reset code."));

        var result = await userManager.ResetPasswordAsync(user, resetUserData.Token, resetUserData.NewPassword);

        if (result.Succeeded is false)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure<AppUserResponse>(new Error(400, errors));
        }

        var userResponse = await GetUserAsync(model.Email);

        return userResponse;
    }

    public async Task<Result<AppUserResponse>> CreateAccessTokenByRefreshTokenAsync()
	{
		var refreshTokenFromCookie = httpContextAccessor.HttpContext!.Request.Cookies["refreshToken"];

		var user = await userManager.Users
			.SingleOrDefaultAsync(u => u.RefreshTokens!.Any(t => t.Token == refreshTokenFromCookie));

		if (user?.RefreshTokens is null)
			return Result.Failure<AppUserResponse>(new Error(401, "Invalid or inactive refresh token."));

		var refreshToken = user.RefreshTokens.Single(t => t.Token == refreshTokenFromCookie);

		if (refreshToken.IsActive is false)
			return Result.Failure<AppUserResponse>(new Error(401, "Invalid or inactive refresh token."));

		refreshToken.RevokedAt = DateTime.UtcNow;

		var newRefreshToken = GenerateRefreshToken();

		user.RefreshTokens.Add(newRefreshToken);

		await userManager.UpdateAsync(user);

		var accessToken = GenerateAccessTokenAsync(user.Id, user.DisplayName, user.Email!);

		var userResponse = new AppUserResponse(user.DisplayName, user.Email!, accessToken, newRefreshToken.ExpireAt);

		SetRefreshTokenInCookie(newRefreshToken.Token, newRefreshToken.ExpireAt);

		return Result.Success(userResponse);
	}

	public async Task<Result<UserAddressResponse>> GetCurrentUserAddress(ClaimsPrincipal userClaims)
	{
		var email = userClaims.FindFirstValue(ClaimTypes.Email);

		var user = await userManager.Users.Include(x => x.Address).SingleOrDefaultAsync(u => u.Email == email);

		if (user?.Address is null)
			return Result.Failure<UserAddressResponse>(new Error(404, "The address is not available in our system."));

		var address = mapper.Map<UserAddress, UserAddressResponse>(user.Address);

		return Result.Success(address);
	}

	public async Task<Result<UserAddressResponse>> UpdateUserAddress(UserAddressResponse updatedAddress, ClaimsPrincipal userClaims)
	{
		var email = userClaims.FindFirstValue(ClaimTypes.Email);

		var address = mapper.Map<UserAddressResponse, UserAddress>(updatedAddress);

		var userEmail = userClaims.FindFirstValue(ClaimTypes.Email);

		var user = await userManager.Users.Include(x => x.Address).SingleOrDefaultAsync(u => u.Email == userEmail);

		user!.Address = address;

		address.AppUserId = user.Id;

		var result = await userManager.UpdateAsync(user);

		if (result.Succeeded)
			return Result.Success(updatedAddress);

		var errors = string.Join(", ", result.Errors.Select(e => e.Description));

		return Result.Failure<UserAddressResponse>(new Error(400, errors));
	}

	public async Task<Result> RevokeRefreshTokenAsync()
	{
		var refreshTokenFromCookie = httpContextAccessor.HttpContext!.Request.Cookies["refreshToken"];

		var user = await userManager.Users
			.SingleOrDefaultAsync(u => u.RefreshTokens!.Any(t => t.Token == refreshTokenFromCookie));

		if (user?.RefreshTokens is null)
			return Result.Failure(new Error(401, "Invalid or inactive refresh token."));

		var refreshToken = user.RefreshTokens.Single(t => t.Token == refreshTokenFromCookie);

		if (refreshToken.IsActive is false)
			return Result.Failure(new Error(401, "Invalid or inactive refresh token."));

		refreshToken.RevokedAt = DateTime.UtcNow;

		await userManager.UpdateAsync(user);

		return Result.Success("Refresh token revoked successfully.");
	}

    private string GenerateAccessTokenAsync(string id, string name, string email)
    {
        var authClaims = new List<Claim>
        {
			new (ClaimTypes.NameIdentifier, id),
			new (ClaimTypes.GivenName, name),
			new (ClaimTypes.Email, email)
		};

        var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jWtData.SecretKey));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jWtData.ValidIssuer,
            Audience = _jWtData.ValidAudience,
            Expires = DateTime.UtcNow.AddMinutes(_jWtData.DurationInMinutes),
            Claims = authClaims.ToDictionary(c => c.Type, object (c) => c.Value),
            SigningCredentials = new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature),
            EncryptingCredentials = new EncryptingCredentials(TokenEncryption.RsaKey, SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes128CbcHmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    private RefreshToken GenerateRefreshToken()
    {
        const int tokenLength = 32;
        var randomNumber = new byte[tokenLength];

        using var generator = RandomNumberGenerator.Create();

        generator.GetBytes(randomNumber);

        var token = Convert.ToBase64String(randomNumber);

        return new RefreshToken
        {
            Token = token,
            ExpireAt = DateTime.UtcNow.AddDays(_jWtData.RefreshTokenExpirationInDays)
        };
    }

    private void SetRefreshTokenInCookie(string token, DateTime expireAt)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = expireAt.ToLocalTime()
        };

        httpContextAccessor.HttpContext!.Response.Cookies.Append("refreshToken", token, cookieOptions);
    }

    private async Task<Result<bool>> Register(string Name, string Email)
    {
        var user = await userManager.FindByEmailAsync(Email);

        if (user is not null)
            return Result.Failure<bool>(new Error(400, "The email address you entered is already taken, Please try a different one."));

        var newUser = new AppUser
        {
            DisplayName = Name,
            Email = Email,
            UserName = Email.Split('@')[0],
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(newUser);

        if (result.Succeeded is false)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));

            return Result.Failure<bool>(new Error(400, errors));
        }

        return Result.Success(true);
    }
}