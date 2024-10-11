using BlazorEcommerce.Application.Dtos;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace BlazorEcommerce.Infrastructure.Services;
public static class AccountServiceHelper
{
	internal static async Task<OAuthTokenResponse?> GetGoogleAccessTokenAsync(string authorizationCode, string clientId, string clientSecret, string baseUrl)
	{
		using var client = new HttpClient();

		var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token");

		var parameters = new Dictionary<string, string>
		{
			{"code", authorizationCode},
			{"client_id", clientId},
			{"client_secret", clientSecret},
			{"redirect_uri", $"{baseUrl}/api/v1/Account/google-response"},
			{"grant_type", "authorization_code"}
		};

		tokenRequest.Content = new FormUrlEncodedContent(parameters);

		var response = await client.SendAsync(tokenRequest);

		if (response.IsSuccessStatusCode is false) return null;

		var json = await response.Content.ReadAsStringAsync();

		var tokenResponse = JsonConvert.DeserializeObject<OAuthTokenResponse>(json);

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

	internal static string LoadEmailTemplate(string filePath, string code, string userName, string title, string message)
	{
		var template = File.ReadAllText(filePath);

		template = template.Replace("{{Code}}", code)
						   .Replace("{{UserName}}", userName)
						   .Replace("{{Title}}", title)
						   .Replace("{{Message}}", message)
						   .Replace("{{Year}}", DateTime.Now.Year.ToString());

		return template;
	}

	internal static string EmailBody(string code, string userName, string title, string message)
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
                            line-height: 1.6;
                            background-color: #f5f5f5;
                            margin: 0;
                            padding: 0;
                        }}
                        .container {{
                            max-width: 600px;
                            margin: auto;
                            padding: 20px;
                            background-color: #ffffff;
                            border-radius: 8px;
                            box-shadow: 0 0 10px rgba(0,0,0,0.1);
                        }}
                        .header {{
                            background-color: #007bff;
                            color: #ffffff;
                            padding: 10px;
                            text-align: center;
                            border-top-left-radius: 8px;
                            border-top-right-radius: 8px;
                        }}
                        .content {{
                            padding: 20px;
                        }}
                        .code {{
                            font-size: 24px;
                            font-weight: bold;
                            text-align: center;
                            margin-top: 20px;
                            margin-bottom: 30px;
                            color: #007bff;
                        }}
                        .footer {{
                            background-color: #f7f7f7;
                            padding: 10px;
                            text-align: center;
                            border-top: 1px solid #dddddd;
                            font-size: 12px;
                            color: #777777;
                        }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""header"">
                            <h2>{title}</h2>
                        </div>
                        <div class=""content"">
                            <p>Dear {userName},</p>
                            <p>{message}, please use the following verification code:</p>
                            <div class=""code"">{code}</div>
                            <p>This code will expire in 5 minutes. Please use it promptly to verify your email address.</p>
                            <p>If you did not request this verification, please ignore this email.</p>
                        </div>
                        <div class=""footer"">
                            <p>&copy; 2024 Mercora. All rights reserved.</p>
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