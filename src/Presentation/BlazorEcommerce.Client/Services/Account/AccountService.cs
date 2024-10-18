using System.Net.Http.Json;
using BlazorEcommerce.Shared.Account;
using Blazored.LocalStorage;

namespace BlazorEcommerce.Client.Services.Account;
public class AccountService(HttpClient httpClient, ILocalStorageService LocalStorage) : IAccountService
{
	public async Task<bool> SendEmailVerification(RegisterVerificationRequest registerVerificationRequest)
	{
		return (await httpClient.PostAsJsonAsync("api/v1/account/send-email-verification-code", registerVerificationRequest)).IsSuccessStatusCode;
	}

	public async Task<AppUserResponse?> Register(RegisterRequest registerRequest)
	{
        try
		{
			var response = await httpClient.PostAsJsonAsync("api/v1/account/register", registerRequest);

			if(!response.IsSuccessStatusCode)
			{
				return null;
			}

			return await response.Content.ReadFromJsonAsync<AppUserResponse>();
		}
		catch
		{
			return null;
		}
	}

	public async Task<AppUserResponse?> GetUserAsync(string email)
	{
		try
		{
			var response = await httpClient.GetAsync($"api/v1/account/get-user?email={email}");

			if (response.IsSuccessStatusCode)
			{
				var result = await response.Content.ReadFromJsonAsync<AppUserResponse>();

				if (result != null && string.IsNullOrEmpty(result.Token) is false)
				{
					Console.WriteLine("Token retrieved: " + result.Token);

					await LocalStorage.SetItemAsync("authToken", result.Token);

					return result;
				}
			}

			return null;
		}
		catch
		{
			return null;
		}
	}

	public async Task<AppUserResponse?> Login(LoginRequest model)
	{
		try
		{
			var response = await httpClient.PostAsJsonAsync("api/v1/account/login", model);

			if (!response.IsSuccessStatusCode)
			{
				return null;
			}

			return await response.Content.ReadFromJsonAsync<AppUserResponse>();
		}
		catch
		{
			return null;
		}
	}
    
	public async Task SendResetPasswordCode(ResetPasswordRequest resetPasswordRequest)
	{
		await httpClient.PostAsJsonAsync("api/v1/account/send-reset-password-code", resetPasswordRequest);
	}
	
	public async Task<AppUserResponse?> ResetPassword(ResetPassword resetPassword)
	{
		try
		{
			var response = await httpClient.PostAsJsonAsync("api/v1/account/reset-password", resetPassword);

			if (!response.IsSuccessStatusCode)
			{
				return null;
			}

			return await response.Content.ReadFromJsonAsync<AppUserResponse>();
		}
		catch
		{
			return null;
		}
	}

}