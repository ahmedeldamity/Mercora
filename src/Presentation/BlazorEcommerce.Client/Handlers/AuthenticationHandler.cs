using System.Net;
using System.Net.Http.Headers;

namespace BlazorEcommerce.Client.Handlers;
public class AuthenticationHandler(ILocalStorageService localStorageService, IServiceProvider serviceProvider) : DelegatingHandler
{
	private bool _refreshing;

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var requestUri = request.RequestUri;

		if (requestUri is not null && (requestUri.AbsolutePath.ToLower().Contains("login") || requestUri.AbsolutePath.ToLower().Contains("register")))
		{
			return await base.SendAsync(request, cancellationToken);
		}

		var accessToken = await localStorageService.GetItemAsync<string>("AuthenticationToken", cancellationToken);

		if (string.IsNullOrEmpty(accessToken) is false)
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Replace("\"", ""));

		var response = await base.SendAsync(request, cancellationToken);

		if (!_refreshing && !string.IsNullOrEmpty(accessToken) && response.StatusCode == HttpStatusCode.Unauthorized)
		{
			try
			{
				_refreshing = true;

				var accountService = serviceProvider.GetRequiredService<IAccountService>();

				var refreshTokensResult = await accountService.TryRefreshTokenAsync();

				if (string.IsNullOrEmpty(refreshTokensResult) is false)
				{
					request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshTokensResult.Replace("\"", ""));

					response = await base.SendAsync(request, cancellationToken);
				}
			}
			finally
			{
				_refreshing = false;
			}
		}

		return response;
	}
}