using Core.Dtos;
using Core.ErrorHandling;
using System.Security.Claims;

namespace Core.Interfaces.Services;
public interface IAccountService
{
    Task<Result<AppUserResponse>> Register(RegisterRequest model);
    Task<Result<AppUserResponseV20>> RegisterV20(RegisterRequest model);
    Task<Result<AppUserResponseV21>> RegisterV21(RegisterRequest model);
    Task<Result<AppUserResponse>> Login(LoginRequest model);
    Task<Result<AppUserResponse>> GetCurrentUser(ClaimsPrincipal User);
    Task<Result<UserAddressResponse>> GetCurrentUserAddress(ClaimsPrincipal User);
    Task<Result<UserAddressResponse>> UpdateUserAddress(UserAddressResponse updatedAddress, ClaimsPrincipal User);
    Task<Result<AppUserResponse>> GoogleLogin(string credential);
    Task<Result<AppUserResponse>> CreateAccessTokenByRefreshTokenAsync();
    Task<Result> RevokeRefreshTokenAsync();

}