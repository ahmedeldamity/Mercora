using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace BlazorEcommerce.Client;
public class CustomAuthenticationStateProvider(HttpClient httpClient, ILocalStorageService localStorageService,
NavigationManager navigationManager) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var savedToken = await localStorageService.GetItemAsync<string>("AuthenticationToken");

        var identity = new ClaimsIdentity();

        httpClient.DefaultRequestHeaders.Authorization = null;

        if (string.IsNullOrEmpty(savedToken) is false)
        {
            try
            {
	            identity = new ClaimsIdentity("jwt");
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken.Replace("\"", ""));
            }
            catch
            {
                await localStorageService.RemoveItemAsync("AuthenticationToken");
                identity = new ClaimsIdentity();
            }
        }

        var user = new ClaimsPrincipal(identity);

        var state = new AuthenticationState(user);

        NotifyAuthenticationStateChanged(Task.FromResult(state));

        return state;
    }

}