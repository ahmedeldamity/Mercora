namespace BlazorEcommerce.Client.Services.Account;
public interface IAccountService
{
	public Task<bool> SendEmailVerification(RegisterVerificationRequest registerVerificationRequest);

    Task<AppUserResponse?> Register(RegisterRequest registerRequest);

	Task<AppUserResponse?> GetUserAsync(string email);

	Task<AppUserResponse?> Login(LoginRequest model);

	Task Logout();

	Task SendResetPasswordCode(ResetPasswordRequest resetPasswordRequest);
	
	Task<AppUserResponse?> ResetPassword(ResetPassword resetPassword);

	Task<string?> TryRefreshTokenAsync();

	Task<UserAddressModel?> GetUserAddressAsync();

	Task<UserAddressModel?> UpdateUserAddressAsync(UserAddressModel userAddressRequest);

	Task GoogleResponse(string code);

    Task GithubResponse(string code);
}