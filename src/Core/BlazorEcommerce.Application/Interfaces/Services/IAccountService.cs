using BlazorEcommerce.Application.Dtos;
using BlazorEcommerce.Domain.ErrorHandling;
using System.Security.Claims;

namespace BlazorEcommerce.Application.Interfaces.Services;
public interface IAccountService
{
    Task<Result<AppUserResponse>> Register(RegisterRequest model);
    Task<Result<AppUserResponseV20>> RegisterV20(RegisterRequest model);
    Task<Result<AppUserResponseV21>> RegisterV21(RegisterRequest model);
    Task<Result<AppUserResponse>> Login(LoginRequest model);
    Task<Result<AppUserResponseV20>> LoginV20(LoginRequest model);
    Task<Result<CurrentUserResponse>> GetCurrentUser(ClaimsPrincipal userClaims);
    Task<Result<UserAddressResponse>> GetCurrentUserAddress(ClaimsPrincipal userClaims);
    Task<Result<UserAddressResponse>> UpdateUserAddress(UserAddressResponse updatedAddress, ClaimsPrincipal userClaims);
    string GoogleLogin();
    Task<Result<AppUserResponse>> GoogleResponse(string code);
	Task<Result<AppUserResponse>> CreateAccessTokenByRefreshTokenAsync();
    Task<Result> RevokeRefreshTokenAsync();
}