using Shared.Dtos;
using Shared.Helpers;
using System.Security.Claims;

namespace Core.Interfaces.Services;
public interface IAccountService
{
    Task<Result<AppUserResponse>> Register(RegisterRequest model);

    Task<Result<AppUserResponse>> Login(LoginRequest model);

    Task<Result<AppUserResponse>> GetCurrentUser(ClaimsPrincipal User);

    Task<Result<UserAddressResponse>> GetCurrentUserAddress(ClaimsPrincipal User);

    Task<Result<UserAddressResponse>> UpdateUserAddress(UserAddressResponse updatedAddress, ClaimsPrincipal User);

    Task<Result<AppUserResponse>> GoogleLogin(string credential);

}