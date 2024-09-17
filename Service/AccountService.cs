using AutoMapper;
using Core.Dtos;
using Core.Entities.IdentityEntities;
using Core.ErrorHandling;
using Core.Interfaces.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Service.ConfigurationData;
using Service.Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Service;
public class AccountService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper,
IOptions<JwtData> jWtData, IOptions<GoogleData> googleData, IHttpContextAccessor httpContextAccessor) : IAccountService
{
    private readonly JwtData _jWtData = jWtData.Value;
    private readonly GoogleData _googleData = googleData.Value;

    public async Task<Result<AppUserResponse>> Register(RegisterRequest model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);

        // Check if the email is already registered and confirmed 
        if (user?.EmailConfirmed is true)
        {
            return Result.Failure<AppUserResponse>(new Error(400, "The email address you entered is already taken, Please try a different one."));
        }

        var newUser = new AppUser()
        {
            DisplayName = model.DisplayName,
            Email = model.Email,
            UserName = model.Email.Split('@')[0],
            PhoneNumber = model.PhoneNumber,
            EmailConfirmed = false
        };

        var result = await userManager.CreateAsync(newUser, model.Password);

        if (result.Succeeded is false)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure<AppUserResponse>(new Error(400, errors));
        }

        var token = await GenerateAccessTokenAsync(newUser);

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
            PhoneNumber = model.PhoneNumber,
            EmailConfirmed = false
        };

        var result = await userManager.CreateAsync(newUser, model.Password);

        if (result.Succeeded is false)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure<AppUserResponseV20>(new Error(400, errors));
        }

        var token = await GenerateAccessTokenAsync(newUser);

        var refreshToken = GenerateRefreshToken();

        var refreshTokenExpireAt = refreshToken.ExpireAt.ToString("MM/dd/yyyy hh:mm");

        var userResponse = new AppUserResponseV20(newUser.DisplayName, newUser.Email, token, newUser.PhoneNumber, refreshTokenExpireAt);

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
            PhoneNumber = model.PhoneNumber,
            EmailConfirmed = false
        };

        var result = await userManager.CreateAsync(newUser, model.Password);

        if (result.Succeeded is false)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure<AppUserResponseV21>(new Error(400, errors));
        }

        var token = await GenerateAccessTokenAsync(newUser);

        var refreshToken = GenerateRefreshToken();

        var refreshTokenExpireAt = refreshToken.ExpireAt.ToString("MM/dd/yyyy hh:mm tt");

        var firstName = model.DisplayName.Trim().Split(' ')[0];

        var lastName = model.DisplayName.Trim().Split(' ')[1];

        var userResponse = new AppUserResponseV21(firstName, lastName, newUser.Email, token, newUser.PhoneNumber, refreshTokenExpireAt);

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

        var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);

        if (result.Succeeded is false)
        {
            var errors = string.Join(", ", "The email or password you entered is incorrect, Check your credentials and try again.");
            return Result.Failure<AppUserResponse>(new Error(400, errors));
        }

        var token = await GenerateAccessTokenAsync(user);

        RefreshToken refreshToken;

        if (user!.RefreshTokens is not null && user.RefreshTokens.Any(t => t.IsActive))
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

        var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);

        if (result.Succeeded is false)
        {
            var errors = string.Join(", ", "The email or password you entered is incorrect, Check your credentials and try again.");
            return Result.Failure<AppUserResponseV20>(new Error(400, errors));
        }

        var token = await GenerateAccessTokenAsync(user);

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

        var userResponse = new AppUserResponseV20(user.DisplayName, user.Email!, token, user.PhoneNumber!, refreshTokenExpireAt);

        SetRefreshTokenInCookie(refreshToken.Token, refreshToken.ExpireAt);

        return Result.Success(userResponse);
    }

    public async Task<Result<CurrentUserResponse>> GetCurrentUser(ClaimsPrincipal userClaims)
    {
        var email = userClaims.FindFirstValue(ClaimTypes.Email);

        var user = await userManager.FindByEmailAsync(email!);

        var token = await GenerateAccessTokenAsync(user!);

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

    public async Task<Result<AppUserResponse>> GoogleLogin(string credential)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = [ _googleData.ClientId ]
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(credential, settings);

        if (string.IsNullOrEmpty(payload.Email))
            return Result.Failure<AppUserResponse>(new Error(400, "Invalid payload: Email is missing."));

        var user = await userManager.FindByEmailAsync(payload.Email);

        if (user is null)
            return Result.Failure<AppUserResponse>(new Error(400, "Your email is not registered, Please register first."));

        user.EmailConfirmed = true;

        await userManager.UpdateAsync(user);

        var token = await GenerateAccessTokenAsync(user);

        RefreshToken refreshToken;

        if (user!.RefreshTokens is not null && user.RefreshTokens.Any(t => t.IsActive))
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

    // access token become not valid so the front-end give me user refresh token to validate it and if it is okay I will generate access token and sent it 
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

        var accessToken = await GenerateAccessTokenAsync(user);

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

        refreshToken.RevokedAt= DateTime.UtcNow;

        await userManager.UpdateAsync(user);

        return Result.Success("Refresh token revoked successfully.");
    }

    private async Task<string> GenerateAccessTokenAsync(AppUser user)
    {

        // Private Claims (user defined - can change from user to others)
        var authClaims = new List<Claim>()
        {
            new (ClaimTypes.GivenName, user.UserName!),
            new (ClaimTypes.Email, user.Email!)
        };

        var userRoles = await userManager.GetRolesAsync(user);

        authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        // secret key
        var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jWtData.SecretKey));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jWtData.ValidIssuer,
            Audience = _jWtData.ValidAudience,
            Expires = DateTime.UtcNow.AddMinutes(_jWtData.DurationInMinutes),
            Claims = authClaims.ToDictionary(c => c.Type, c => (object)c.Value),
            SigningCredentials = new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature),
            EncryptingCredentials = new EncryptingCredentials(TokenEncryption.RsaKey, SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes128CbcHmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Create and return the encrypted JWT (JWE)
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

}