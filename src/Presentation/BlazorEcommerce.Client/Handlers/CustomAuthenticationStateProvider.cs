using System.Security.Claims;

namespace BlazorEcommerce.Client.Handlers;
public class CustomAuthenticationStateProvider(ILocalStorageService localStorageService) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var savedToken = await localStorageService.GetItemAsync<string>("AuthenticationToken");

        var identity = new ClaimsIdentity();

        if (string.IsNullOrEmpty(savedToken) is false)
            identity = new ClaimsIdentity("jwt");

        var user = new ClaimsPrincipal(identity);

        var state = new AuthenticationState(user);

        NotifyAuthenticationStateChanged(Task.FromResult(state));

        return state;
    }

}