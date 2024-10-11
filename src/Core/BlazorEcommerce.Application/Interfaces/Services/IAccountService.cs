using BlazorEcommerce.Application.Dtos;
using BlazorEcommerce.Domain.ErrorHandling;
using System.Security.Claims;

namespace BlazorEcommerce.Application.Interfaces.Services;
public interface IAccountService
{
	Task<Result> SendEmailVerificationCode(string email, int version = 1, bool forRegister = true);
	Task<Result<AppUserResponse>> VerifyCodeForRegister(RegisterCodeVerificationRequest model);
	Task<Result<AppUserResponse>> VerifyCodeForLogin(LoginCodeVerificationRequest model);
    Task<Result<CurrentUserResponse>> GetCurrentUser(ClaimsPrincipal userClaims);
    Task<Result<UserAddressResponse>> GetCurrentUserAddress(ClaimsPrincipal userClaims);
    Task<Result<UserAddressResponse>> UpdateUserAddress(UserAddressResponse updatedAddress, ClaimsPrincipal userClaims);
    string GoogleLogin();
	Task<Result<AppUserResponse>> CreateAccessTokenByRefreshTokenAsync();
    Task<Result> RevokeRefreshTokenAsync();
}