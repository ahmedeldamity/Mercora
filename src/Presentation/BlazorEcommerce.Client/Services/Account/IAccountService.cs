using BlazorEcommerce.Shared.Account;

namespace BlazorEcommerce.Client.Services.Account;
public interface IAccountService
{
	public Task<bool> SendEmailVerification(RegisterVerificationRequest registerVerificationRequest);

    Task<AppUserResponse?> Register(RegisterRequest registerRequest);

	Task<AppUserResponse?> GetUserAsync(string email);

	Task<AppUserResponse?> Login(LoginRequest model);

	Task SendResetPasswordCode(ResetPasswordRequest resetPasswordRequest);
	
	Task<AppUserResponse?> ResetPassword(ResetPassword resetPassword);
}