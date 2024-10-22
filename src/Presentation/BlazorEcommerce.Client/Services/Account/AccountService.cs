using System.Net;
using System.Net.Http.Headers;

namespace BlazorEcommerce.Client.Services.Account;
public class AccountService(IHttpClientFactory _httpClientFactory, ILocalStorageService LocalStorage) : IAccountService
{
	private readonly HttpClient httpClient = _httpClientFactory.CreateClient("Auth");

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
					return result;
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

	public async Task Logout()
	{
		await LocalStorage.RemoveItemAsync("AuthenticationToken");

		await httpClient.PostAsync("/api/account/v1/revoke-token", null);
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

	public async Task<string?> TryRefreshTokenAsync()
	{
		var response = await httpClient.GetAsync("api/v1/account/refresh-token");

		if (response.IsSuccessStatusCode)
		{
			var result = await response.Content.ReadFromJsonAsync<AppUserResponse>();

			if (result != null && string.IsNullOrEmpty(result.Token) is false)
			{
				await LocalStorage.SetItemAsync("AuthenticationToken", result.Token);

				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);

				return result.Token;
			}
		}

		await Logout();

		return null;
	}

	public async Task<UserAddressModel?> GetUserAddressAsync()
	{
		try
		{
			var response = await httpClient.GetAsync("api/v1/account/get-current-user-address");

			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadFromJsonAsync<UserAddressModel>();
			}

			return null;
		}
		catch
		{
			return null;
		}
	}

	public async Task<UserAddressModel?> UpdateUserAddressAsync(UserAddressModel userAddressRequest)
	{
		try
		{
			var response = await httpClient.PutAsJsonAsync("api/v1/account/update-current-user-address", userAddressRequest);

			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadFromJsonAsync<UserAddressModel>();
			}

			return null;
		}
		catch
		{
			return null;
		}
	}

}