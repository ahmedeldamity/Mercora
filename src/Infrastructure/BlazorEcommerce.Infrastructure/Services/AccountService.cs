using AutoMapper;
using BlazorEcommerce.Application.Dtos;
using BlazorEcommerce.Application.Interfaces.Repositories;
using BlazorEcommerce.Application.Interfaces.Services;
using BlazorEcommerce.Application.Models;
using BlazorEcommerce.Application.Specifications.Identity;
using BlazorEcommerce.Domain.Entities.IdentityEntities;
using BlazorEcommerce.Domain.ErrorHandling;
using BlazorEcommerce.Infrastructure.Utility;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BlazorEcommerce.Infrastructure.Services;
public class AccountService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper, 
IEmailSettingService emailSettings, IOptions<JwtData> jWtData, IOptions<GoogleData> googleData, IHttpContextAccessor httpContextAccessor,
IOptions<Urls> urls, IUnitOfWork unitOfWork) : IAccountService
{
    private readonly JwtData _jWtData = jWtData.Value;
    private readonly GoogleData _googleData = googleData.Value;
    private readonly Urls _urls = urls.Value;

    public async Task<Result> SendEmailVerificationCode(string email, bool forRegister = true)
	{
		if (forRegister)
		{
			var user = await userManager.FindByEmailAsync(email);

			if (user is not null)
				return Result.Failure<AppUserResponse>(new Error(400, "The email address you entered is already taken, Please try a different one."));
		}

		var code = GenerateSecureCode();

		var subject = $"✅ {email.Split('@')[0]}, Your pin code is {code}. \r\nPlease confirm your email address";

		var body = EmailBody(code, email.Split('@')[0], "Email Verification", "Thank you for registering with our service. To complete your registration");

		EmailResponse emailToSend = new(subject, body, email);
		
		var identityCode = new IdentityCode
		{
			Code = HashCode(code),
			IsActive = true,
			Email = email,
			ForRegistrationConfirmed = forRegister
		};

		await unitOfWork.Repository<IdentityCode>().AddAsync(identityCode);

		await unitOfWork.CompleteAsync();

		BackgroundJob.Enqueue(() => emailSettings.SendEmailMessage(emailToSend));

		return Result.Success();
	}

	public async Task<Result> SendEmailVerificationCodeV2(string email, bool forRegister = true)
	{
		if (forRegister)
		{
			var user = await userManager.FindByEmailAsync(email);

			if (user is not null)
				return Result.Failure<AppUserResponse>(new Error(400, "The email address you entered is already taken, Please try a different one."));
		}

		var code = GenerateSecureCode();

		var subject = $"✅ {email!.Split('@')[0]}, Your pin code is {code}. \r\nPlease confirm your email address";

		var body = LoadEmailTemplate("Templates/EmailTemplate.html", code, email.Split('@')[0], "Reset Password", "You have requested to reset your password.");

		EmailResponse emailToSend = new(subject, body, email);

		await unitOfWork.Repository<IdentityCode>().AddAsync(new IdentityCode
		{
			Code = HashCode(code),
			IsActive = true,
			Email = email,
			ForRegistrationConfirmed = forRegister
		});

		await unitOfWork.CompleteAsync();

		BackgroundJob.Enqueue(() => emailSettings.SendEmailMessage(emailToSend));

		return Result.Success("A email verification code has been successfully sent.");
	}

	public async Task<Result<AppUserResponse>> VerifyCode(CodeVerificationRequest model, bool forRegister = true)
	{
		if (forRegister)
		{
			var user = await userManager.FindByEmailAsync(model.Email);

			if (user is not null)
				return Result.Failure<AppUserResponse>(new Error(400, "The email address you entered is already taken, Please try a different one."));
		}

		var spec = new IdentityCodeSpecification(model.Email, forRegister);

		var identityCodes = await unitOfWork.Repository<IdentityCode>().GetAllAsync(spec);

		var identityCode = identityCodes.LastOrDefault();

		if (identityCode is null)
			return Result.Failure<AppUserResponse>(new Error(400, "The reset code is missing or invalid. Please request a new reset code."));

		var lastCode = identityCode.Code;

		if (!ConstantComparison(lastCode, HashCode(model.VerificationCode)))
			return Result.Failure<AppUserResponse>(new Error(400, "The reset code is missing or invalid. Please request a new reset code."));

		if (!identityCode.IsActive || identityCode.CreationTime.Minute + 5 < DateTime.UtcNow.Minute)
			return Result.Failure<AppUserResponse>(new Error(400, "The reset code has either expired or is not active. Please request a new code."));

		identityCode.IsActive = false;

		unitOfWork.Repository<IdentityCode>().Update(identityCode);

		await unitOfWork.CompleteAsync();

		Result<AppUserResponse>? result;

		if (forRegister)
		{
            var registerRequest = new RegisterRequest(model.DisplayName, model.Email);

			result = await Register(registerRequest);

			return result;
		}

		var loginRequest = new LoginRequest(model.Email);

		result = await Login(loginRequest);

		return result;
	}

	public async Task<Result<AppUserResponse>> Register(RegisterRequest model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);

        if (user is not null)
        {
            return Result.Failure<AppUserResponse>(new Error(400, "The email address you entered is already taken, Please try a different one."));
        }

        var newUser = new AppUser
        {
            DisplayName = model.DisplayName,
            Email = model.Email,
            UserName = model.Email.Split('@')[0],
            EmailConfirmed = true
        };

		var result = await userManager.CreateAsync(newUser);

		if (result.Succeeded is false)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure<AppUserResponse>(new Error(400, errors));
        }

        var token = GenerateAccessTokenAsync(newUser.Id, newUser.DisplayName, newUser.Email);

        var refreshToken = GenerateRefreshToken();

        var userResponse = new AppUserResponse(newUser.DisplayName, newUser.Email, token, refreshToken.ExpireAt);

        newUser.RefreshTokens!.Add(refreshToken);

        await userManager.UpdateAsync(newUser);

        SetRefreshTokenInCookie(refreshToken.Token, refreshToken.ExpireAt);

        return Result.Success(userResponse);
    }

    public async Task<Result<AppUserResponseV20>> RegisterV20(RegisterRequest model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);

        // Check if the email is already registered and confirmed 
        if (user?.EmailConfirmed is true)
        {
            return Result.Failure<AppUserResponseV20>(new Error(400, "The email address you entered is already taken, Please try a different one."));
        }

        var newUser = new AppUser()
        {
            DisplayName = model.DisplayName,
            Email = model.Email,
            UserName = model.Email.Split('@')[0],
            EmailConfirmed = false
        };

        //var result = await userManager.CreateAsync(newUser, model.Password);

        var result = await userManager.CreateAsync(newUser);

        if (result.Succeeded is false)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure<AppUserResponseV20>(new Error(400, errors));
        }

		var token = GenerateAccessTokenAsync(newUser.Id, newUser.DisplayName, newUser.Email);

		var refreshToken = GenerateRefreshToken();

        var refreshTokenExpireAt = refreshToken.ExpireAt.ToString("MM/dd/yyyy hh:mm");

        var userResponse = new AppUserResponseV20(newUser.DisplayName, newUser.Email, token, refreshTokenExpireAt);

        newUser.RefreshTokens!.Add(refreshToken);

        await userManager.UpdateAsync(newUser);

        SetRefreshTokenInCookie(refreshToken.Token, refreshToken.ExpireAt);

        return Result.Success(userResponse);
    }

    public async Task<Result<AppUserResponseV21>> RegisterV21(RegisterRequest model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);

        // Check if the email is already registered and confirmed 
        if (user?.EmailConfirmed is true)
        {
            return Result.Failure<AppUserResponseV21>(new Error(400, "The email address you entered is already taken, Please try a different one."));
        }

        var newUser = new AppUser()
        {
            DisplayName = model.DisplayName,
            Email = model.Email,
            UserName = model.Email.Split('@')[0],
            EmailConfirmed = false
        };

		//var result = await userManager.CreateAsync(newUser, model.Password);

		var result = await userManager.CreateAsync(newUser);

		if (result.Succeeded is false)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure<AppUserResponseV21>(new Error(400, errors));
        }

        var token = GenerateAccessTokenAsync(newUser.Id, newUser.DisplayName, newUser.Email);

        var refreshToken = GenerateRefreshToken();

        var refreshTokenExpireAt = refreshToken.ExpireAt.ToString("MM/dd/yyyy hh:mm tt");

        var firstName = model.DisplayName.Trim().Split(' ')[0];

        var lastName = model.DisplayName.Trim().Split(' ')[1];

        var userResponse = new AppUserResponseV21(firstName, lastName, newUser.Email, token, refreshTokenExpireAt);

        newUser.RefreshTokens!.Add(refreshToken);

        await userManager.UpdateAsync(newUser);

        SetRefreshTokenInCookie(refreshToken.Token, refreshToken.ExpireAt);

        return Result.Success(userResponse);
    }
	
    public async Task<Result<AppUserResponse>> Login(LoginRequest model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);
	    
        if (user is null)
            return Result.Failure<AppUserResponse>(new Error(400, "The email or password you entered is incorrect, Check your credentials and try again."));

        var token = GenerateAccessTokenAsync(user.Id, user.DisplayName, user.Email!);

        RefreshToken refreshToken;

        if (user.RefreshTokens is not null && user.RefreshTokens.Any(t => t.IsActive))
        {
            refreshToken = user.RefreshTokens.First(t => t.IsActive);
        }
        else
        {
            refreshToken = GenerateRefreshToken();
            user.RefreshTokens!.Add(refreshToken);
            await userManager.UpdateAsync(user);
        }

        var userResponse = new AppUserResponse(user.DisplayName, user.Email!, token, refreshToken.ExpireAt);

        SetRefreshTokenInCookie(refreshToken.Token, refreshToken.ExpireAt);

        return Result.Success(userResponse);
    }

    public async Task<Result<AppUserResponseV20>> LoginV20(LoginRequest model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);

        if (user is null)
            return Result.Failure<AppUserResponseV20>(new Error(400, "The email or password you entered is incorrect, Check your credentials and try again."));

        var token = GenerateAccessTokenAsync(user.Id, user.DisplayName, user.Email!);

        RefreshToken refreshToken;

        if (user.RefreshTokens is not null && user.RefreshTokens.Any(t => t.IsActive))
        {
            refreshToken = user.RefreshTokens.First(t => t.IsActive);
        }
        else
        {
            refreshToken = GenerateRefreshToken();
            user.RefreshTokens!.Add(refreshToken);
            await userManager.UpdateAsync(user);
        }

        var refreshTokenExpireAt = refreshToken.ExpireAt.ToString("MM/dd/yyyy hh:mm tt");

        var userResponse = new AppUserResponseV20(user.DisplayName, user.Email!, token, refreshTokenExpireAt);

        SetRefreshTokenInCookie(refreshToken.Token, refreshToken.ExpireAt);

        return Result.Success(userResponse);
    }

    public async Task<Result<CurrentUserResponse>> GetCurrentUser(ClaimsPrincipal userClaims)
    {
        var email = userClaims.FindFirstValue(ClaimTypes.Email);

        var user = await userManager.FindByEmailAsync(email!);

		var token = GenerateAccessTokenAsync(user!.Id, user.DisplayName, user.Email!);

		var userResponse = new CurrentUserResponse(user!.DisplayName, user.Email!, token);

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

    public string GoogleLogin()
    {
	    var googleOAuthUrl = $"https://accounts.google.com/o/oauth2/v2/auth/oauthchooseaccount?" +
	                         $"client_id={_googleData.ClientId}" +
	                         $"&redirect_uri={_urls.BaseUrl}/api/v1/Account/google-response" +
	                         $"&response_type=code" +
	                         $"&scope=openid%20profile%20email";

		return googleOAuthUrl;
    }

    public async Task<Result<AppUserResponse>> GoogleResponse(string code)
    {
	    if (string.IsNullOrEmpty(code))
		    return Result.Failure<AppUserResponse>(new Error(400, "Authorization code is missing."));

	    var tokenResponse = await GetGoogleAccessTokenAsync(code);

	    if (tokenResponse == null)
		    return Result.Failure<AppUserResponse>(new Error(400, "Failed to get access token from Google."));

	    var googleUserInfo = await GetGoogleUserInfoAsync(tokenResponse.Access_Token);

	    if (googleUserInfo == null)
		    return Result.Failure<AppUserResponse>(new Error(400, "Failed to get user information from Google."));
			
		var user = await userManager.FindByEmailAsync(googleUserInfo.Email);

		if (user == null)
		{
            var model = new RegisterRequest(googleUserInfo.Name, googleUserInfo.Email);

            return await Register(model);
		}
		else
		{
			var model = new LoginRequest(googleUserInfo.Email, "P@ssw0rd");

			return await Login(model);
		}
    }

    private async Task<OAuthTokenResponse?> GetGoogleAccessTokenAsync(string authorizationCode)
    {
	    using var client = new HttpClient();

	    var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token");

	    var parameters = new Dictionary<string, string>
	    {
		    {"code", authorizationCode},
		    {"client_id", _googleData.ClientId},
		    {"client_secret", _googleData.ClientSecret},
		    {"redirect_uri", $"{_urls.BaseUrl}/api/v1/Account/google-response"},
		    {"grant_type", "authorization_code"}
	    };

	    tokenRequest.Content = new FormUrlEncodedContent(parameters);

	    var response = await client.SendAsync(tokenRequest);

	    if (response.IsSuccessStatusCode is false) return null;

	    var json = await response.Content.ReadAsStringAsync();

	    var tokenResponse = JsonConvert.DeserializeObject<OAuthTokenResponse>(json);

	    return tokenResponse;
    }

    private static async Task<GoogleUserInformation?> GetGoogleUserInfoAsync(string accessToken)
    {
	    using var client = new HttpClient();

	    var request = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v3/userinfo");

	    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

	    var response = await client.SendAsync(request);

	    if (response.IsSuccessStatusCode is false) return null;

	    var json = await response.Content.ReadAsStringAsync();

	    return JsonConvert.DeserializeObject<GoogleUserInformation>(json);
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
	
    private static string LoadEmailTemplate(string filePath, string code, string userName, string title, string message)
	{
		var template = File.ReadAllText(filePath);

		template = template.Replace("{{Code}}", code)
						   .Replace("{{UserName}}", userName)
						   .Replace("{{Title}}", title)
						   .Replace("{{Message}}", message)
						   .Replace("{{Year}}", DateTime.Now.Year.ToString());

		return template;
	}
	
    private static string EmailBody(string code, string userName, string title, string message)
	{
		return $@"
                <!DOCTYPE html>
                <html lang=""en"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Email Verification</title>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            line-height: 1.6;
                            background-color: #f5f5f5;
                            margin: 0;
                            padding: 0;
                        }}
                        .container {{
                            max-width: 600px;
                            margin: auto;
                            padding: 20px;
                            background-color: #ffffff;
                            border-radius: 8px;
                            box-shadow: 0 0 10px rgba(0,0,0,0.1);
                        }}
                        .header {{
                            background-color: #007bff;
                            color: #ffffff;
                            padding: 10px;
                            text-align: center;
                            border-top-left-radius: 8px;
                            border-top-right-radius: 8px;
                        }}
                        .content {{
                            padding: 20px;
                        }}
                        .code {{
                            font-size: 24px;
                            font-weight: bold;
                            text-align: center;
                            margin-top: 20px;
                            margin-bottom: 30px;
                            color: #007bff;
                        }}
                        .footer {{
                            background-color: #f7f7f7;
                            padding: 10px;
                            text-align: center;
                            border-top: 1px solid #dddddd;
                            font-size: 12px;
                            color: #777777;
                        }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""header"">
                            <h2>{title}</h2>
                        </div>
                        <div class=""content"">
                            <p>Dear {userName},</p>
                            <p>{message}, please use the following verification code:</p>
                            <div class=""code"">{code}</div>
                            <p>This code will expire in 5 minutes. Please use it promptly to verify your email address.</p>
                            <p>If you did not request this verification, please ignore this email.</p>
                        </div>
                        <div class=""footer"">
                            <p>&copy; 2024 OnionStore. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";
	}

	private static string GenerateSecureCode()
	{
		var randomNumber = new byte[4];

		using var rng = RandomNumberGenerator.Create();
		rng.GetBytes(randomNumber);

		var result = BitConverter.ToInt32(randomNumber, 0);
		var positiveResult = Math.Abs(result);

		var sixDigitCode = positiveResult % 1000000;
		return sixDigitCode.ToString("D6");
	}

	private static string HashCode(string code)
	{
		var sha256 = SHA256.Create();
		var hashedBytes = sha256?.ComputeHash(Encoding.UTF8.GetBytes(code));
		return BitConverter.ToString(hashedBytes!).Replace("-", "");
	}

	private static bool ConstantComparison(string a, string b)
	{
		if (a.Length != b.Length)
			return false;

		var result = 0;
		for (var i = 0; i < a.Length; i++)
		{
			result |= a[i] ^ b[i];
		}
		return result == 0;
	}
}