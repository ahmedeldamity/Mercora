using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace BlazorEcommerce.Infrastructure.Services;
public static class AccountServiceHelper
{
	internal static async Task<OAuthGoogleTokenResponse?> GetGoogleAccessTokenAsync(string authorizationCode, string clientId, string clientSecret, string baseUrl)
	{
		using var client = new HttpClient();

		var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token");

		var parameters = new Dictionary<string, string>
		{
			{"code", authorizationCode},
			{"client_id", clientId},
			{"client_secret", clientSecret},
			{"redirect_uri", $"{baseUrl}/google-oauth-code"},
			{"grant_type", "authorization_code"}
		};

		tokenRequest.Content = new FormUrlEncodedContent(parameters);

		var response = await client.SendAsync(tokenRequest);

		if (response.IsSuccessStatusCode is false) return null;

		var json = await response.Content.ReadAsStringAsync();

		var tokenResponse = JsonConvert.DeserializeObject<OAuthGoogleTokenResponse>(json);

		return tokenResponse;
	}

	internal static async Task<GoogleUserInformation?> GetGoogleUserInfoAsync(string accessToken)
	{
		using var client = new HttpClient();

		var request = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v3/userinfo");

		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

		var response = await client.SendAsync(request);

		if (response.IsSuccessStatusCode is false) return null;

		var json = await response.Content.ReadAsStringAsync();

		return JsonConvert.DeserializeObject<GoogleUserInformation>(json);
	}

	internal static async Task<OAuthGithubTokenResponse?> GetGitHubAccessTokenAsync(string authorizationCode, string clientId, string clientSecret)
	{
		var client = new HttpClient();

		var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token");
		
		var parameters = new Dictionary<string, string>
		{
			{"client_id", clientId},
			{"client_secret", clientSecret},
			{"code", authorizationCode}
		};

		tokenRequest.Content = new FormUrlEncodedContent(parameters);

		tokenRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

		var response = await client.SendAsync(tokenRequest);

		if (!response.IsSuccessStatusCode) return null;

		var json = await response.Content.ReadAsStringAsync();

		var tokenResponse = JsonConvert.DeserializeObject<OAuthGithubTokenResponse>(json);

		return tokenResponse;
	}

	internal static async Task<GithubUserInformation?> GetGitHubUserInfoAsync(string accessToken)
	{
		var client = new HttpClient();

		var userInfo = await GetGitHubApiResponseAsync(client, "https://api.github.com/user", accessToken) as JObject;

		var emails = await GetGitHubApiResponseAsync(client, "https://api.github.com/user/emails", accessToken) as JArray;

		if (userInfo == null || emails == null) return null;

		var userInformation = userInfo.ToObject<GithubUserInformation>();

		if (userInformation == null) return null;

		var primaryEmail = emails.FirstOrDefault(e => e["primary"]?.Value<bool>() == true);

		userInformation.Email = primaryEmail?["email"]?.ToString() ?? userInformation.Name;

		return userInformation;
	}

	private static async Task<JToken?> GetGitHubApiResponseAsync(HttpClient client, string url, string accessToken)
	{
		var request = new HttpRequestMessage(HttpMethod.Get, url);

		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

		request.Headers.UserAgent.ParseAdd("Mozilla/5.0");

		var response = await client.SendAsync(request);

		if (response.IsSuccessStatusCode is false) return null;

		var json = await response.Content.ReadAsStringAsync();

		return JToken.Parse(json);
	}

	internal static string RegisterEmailBody(string verificationUrl, string userName)
	{
		return $@"
        <!DOCTYPE html>
        <html lang=""en"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>Email Verification</title>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f5f5f5; /* Light background */
                        color: #333; /* Dark text */
                        text-align: center;
                        padding: 40px;
                        margin: 0;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 0 auto;
                        padding: 30px;
                        background-color: #ffffff; /* White background for the email */
                        border-radius: 10px;
                        box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                    }}
                    .header {{
                        font-size: 24px;
                        font-weight: bold;
                        margin-bottom: 20px;
                        color: #1e90ff; /* Light blue color */
                    }}
                    .message {{
                        font-size: 16px;
                        margin-bottom: 30px;
                        line-height: 1.5;
                    }}
                    .verify-btn {{
                        display: inline-block;
                        background-color: #1e90ff; /* Button color */
                        color: white !important;
                        padding: 15px 30px;
                        text-decoration: none;
                        border-radius: 5px;
                        font-size: 18px;
                        font-weight: bold;
                    }}
                    .verify-btn:hover {{
                        background-color: #00bfff; /* Button hover color */
                    }}
                    .disclaimer {{
                        font-size: 12px;
                        margin-top: 30px;
                        color: #777; /* Lighter text color for disclaimer */
                    }}
                    .disclaimer a {{
                        color: #1e90ff; /* Link color */
                        text-decoration: none;
                    }}
                    .space-image {{
                        width: 100px;
                        height: auto;
                        margin-bottom: 20px;
                    }}
                </style>
            </head>
            <body>
                <div class=""container"">
                    <div class=""header"">Verify Your Email Address</div>
                    <div class=""message"">Hi {userName},<br><br>Thank you for registering with our service. To complete your registration verify your email address.</div>
                    <a href=""{verificationUrl}"" class=""verify-btn"">Verify Email Address</a>
                    <div class=""disclaimer"">
                        This link will expire after 3 hours. If you did not make this request, please disregard this email. For assistance, visit our <a href=""https://yourdomain.com/help"">Help Center</a>.
                    </div>
                </div>
            </body>
        </html>";
	}

    internal static string ResetPasswordEmailBody(string verificationUrl, string userName)
    {
        return $@"
        <!DOCTYPE html>
        <html lang=""en"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>Reset Password</title>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f5f5f5; /* Light background */
                        color: #333; /* Dark text */
                        text-align: center;
                        padding: 40px;
                        margin: 0;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 0 auto;
                        padding: 30px;
                        background-color: #ffffff; /* White background for the email */
                        border-radius: 10px;
                        box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                    }}
                    .header {{
                        font-size: 24px;
                        font-weight: bold;
                        margin-bottom: 20px;
                        color: #1e90ff; /* Light blue color */
                    }}
                    .message {{
                        font-size: 16px;
                        margin-bottom: 30px;
                        line-height: 1.5;
                    }}
                    .verify-btn {{
                        display: inline-block;
                        background-color: #1e90ff; /* Button color */
                        color: white !important;
                        padding: 15px 30px;
                        text-decoration: none;
                        border-radius: 5px;
                        font-size: 18px;
                        font-weight: bold;
                    }}
                    .verify-btn:hover {{
                        background-color: #00bfff; /* Button hover color */
                    }}
                    .disclaimer {{
                        font-size: 12px;
                        margin-top: 30px;
                        color: #777; /* Lighter text color for disclaimer */
                    }}
                    .disclaimer a {{
                        color: #1e90ff; /* Link color */
                        text-decoration: none;
                    }}
                    .space-image {{
                        width: 100px;
                        height: auto;
                        margin-bottom: 20px;
                    }}
                </style>
            </head>
            <body>
                <div class=""container"">
                    <div class=""header"">Reset Your Password</div>
                    <div class=""message"">Hi {userName},<br><br>To continue exploring our universe, please verify that this is your email address.</div>
                    <a href=""{verificationUrl}"" class=""verify-btn"">Reset Your Password</a>
                    <div class=""disclaimer"">
                        This link will expire after 3 hours. If you did not make this request, please disregard this email. For assistance, visit our <a href=""https://yourdomain.com/help"">Help Center</a>.
                    </div>
                </div>
            </body>
        </html>";
    }

    internal static string GenerateSecureCode()
	{
		var randomNumber = new byte[4];

		using var rng = RandomNumberGenerator.Create();
		rng.GetBytes(randomNumber);

		var result = BitConverter.ToInt32(randomNumber, 0);
		var positiveResult = Math.Abs(result);

		var sixDigitCode = positiveResult % 1000000;
		return sixDigitCode.ToString("D6");
	}

	internal static string HashCode(string code)
	{
		var sha256 = SHA256.Create();
		var hashedBytes = sha256?.ComputeHash(Encoding.UTF8.GetBytes(code));
		return BitConverter.ToString(hashedBytes!).Replace("-", "");
	}

	internal static bool ConstantComparison(string a, string b)
	{
		if (a.Length != b.Length)
			return false;

		var result = 0;
		for (var i = 0; i < a.Length; i++)
		{
			result |= a[i] ^ b[i];
		}
		return result == 0;
	}
}