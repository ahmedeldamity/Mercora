using BlazorEcommerce.Application.Dtos;
using BlazorEcommerce.Domain.ErrorHandling;
using System.Security.Claims;
using BlazorEcommerce.Shared.Account;

namespace BlazorEcommerce.Application.Interfaces.Services;
public interface IAccountService
{
    Task<Result> SendEmailVerificationCode(RegisterVerificationRequest request);
    Task<Result<AppUserResponse>> VerifyCodeForRegister(RegisterRequest registerRequest);
    Task<Result<AppUserResponse>> GetUserAsync(string email);
    Task<Result<AppUserResponse>> Login(LoginRequest model);
	Task<Result<UserAddressResponse>> GetCurrentUserAddress(ClaimsPrincipal userClaims);
    Task<Result<UserAddressResponse>> UpdateUserAddress(UserAddressResponse updatedAddress, ClaimsPrincipal userClaims);
    string GoogleLogin();
    Task<Result<string>> GoogleResponse(string code);
    string GithubLogin();
    Task<Result<string>> GithubResponse(string code);
    Task<Result> SendResetPasswordCode(ResetPasswordRequest request);
    Task<Result<AppUserResponse>> ResetPassword(ResetPassword resetUserData);
    Task<Result<AppUserResponse>> CreateAccessTokenByRefreshTokenAsync();
    Task<Result> RevokeRefreshTokenAsync();
}