namespace BlazorEcommerce.Application.Interfaces.Services;
public interface IAccountService
{
    Task<Result> SendEmailVerificationCode(RegisterVerificationRequest request);
    Task<Result<AppUserResponse>> VerifyCodeForRegister(RegisterRequest registerRequest);
    Task<Result<AppUserResponse>> GetUserAsync(string email);
    Task<Result<AppUserResponse>> Login(LoginRequest model);
	Task<Result<UserAddressModel>> GetCurrentUserAddress(ClaimsPrincipal userClaims);
    Task<Result<UserAddressModel>> UpdateUserAddress(UserAddressModel updatedAddress, ClaimsPrincipal userClaims);
    string GoogleLogin();
    Task<Result<string>> GoogleResponse(string code);
    string GithubLogin();
    Task<Result<string>> GithubResponse(string code);
    Task<Result> SendResetPasswordCode(ResetPasswordRequest request);
    Task<Result<AppUserResponse>> ResetPassword(ResetPassword resetUserData);
    Task<Result<AppUserResponse>> CreateAccessTokenByRefreshTokenAsync();
    Task<Result> RevokeRefreshTokenAsync();
}