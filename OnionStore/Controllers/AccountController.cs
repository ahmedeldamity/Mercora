﻿using Shared.Dtos;
using API.Errors;
using Core.Entities.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using API.EmailSetting;
using Repository.Identity;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AutoMapper;

namespace API.Controllers;

public class AccountController(UserManager<AppUser> _userManager, SignInManager<AppUser> _signInManager, IMapper _mapper,
             IdentityContext _identityContext, IEmailSettings _emailSettings, ILogger<AccountController> _logger, IAuthService _authService) : BaseController
{

	[HttpPost("register")]
	[ProducesResponseType(typeof(AppUserResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
	public async Task<ActionResult> Register(RegisterRequest model)
	{
		var user = await _userManager.FindByEmailAsync(model.Email);

        // Check if the email is already registered and confirmed 
        if (user is not null && user.EmailConfirmed is true) 
        {
            List<string> errors = ["This email has already been used."];
            return BadRequest(new ApiValidationErrorResponse { Errors = errors });
        }

		var newUser = new AppUser()
		{
			DisplayName = model.DisplayName,
			Email = model.Email,
			UserName = model.Email.Split('@')[0],
			PhoneNumber = model.PhoneNumber,
            EmailConfirmed = false
        };

		var result = await _userManager.CreateAsync(newUser, model.Password);

        if (result.Succeeded is false)
        {
            var error = result.Errors.Select(e => e.Description).FirstOrDefault();
            return BadRequest(new ApiResponse(400, error));
        }

        var token = await _authService.CreateTokenAsync(newUser, _userManager);

		return Ok(new AppUserResponse
		{
			DisplayName = newUser.DisplayName,
			Email = newUser.Email,
			Token = token
		});
	}

    [HttpPost("sendemailverificationcode")]
    [ProducesResponseType(typeof(AppUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AppUserResponse>> SendEmailVerificationCode(EmailRequest emailRequest)
    {
        if (await _userManager.FindByEmailAsync(emailRequest.Email) is not { } user)
            return Ok(new ApiResponse(200, "If your email is registered with us, a email verification code has been successfully sent."));

        var code = GenerateSecureCode();

        EmailResponse emailToSend = new()
        {
            To = emailRequest.Email,
            Subject = $"{emailRequest.Email.Split('@')[0]}, Your pin code is {code}. \r\nPlease confirm your email address",
            Body = EmailBody(code, emailRequest.Email.Split('@')[0], "Email Verification", "Thank you for registering with our service. To complete your registration")
        };

        try
        {
            await _identityContext.IdentityCodes.AddAsync(new IdentityCode()
            {
                Code = HashCode(code),
                IsActive = true,
                User = user,
                AppUserId = user.Id,
                ForRegisterationConfirmed = true
            });

            await _identityContext.SaveChangesAsync();

            await _emailSettings.SendEmailMessage(emailToSend);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while sending password reset email.");
            return StatusCode(500, new ApiResponse(500, "An error occurred while processing your request."));
        }

        return Ok(new ApiResponse(200, "If your email is registered with us, a email verification code has been successfully sent."));
    }

    [ProducesResponseType(typeof(AppUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [HttpPost("verifyregistercode")]
    public async Task<ActionResult> VerifyRegisterCode(CodeVerificationRequest model)
    {
        if(await _userManager.FindByEmailAsync(model.Email) is not { } user)
            return BadRequest(new ApiResponse(400, "Invalid Email."));

        var identityCode = await _identityContext.IdentityCodes
                            .Where(P => P.AppUserId == user.Id && P.ForRegisterationConfirmed)
                            .OrderBy(d => d.CreationTime)
                            .LastOrDefaultAsync();

        if (identityCode is null)
            return BadRequest(new ApiResponse(400, "No valid reset code found."));

        var lastCode = identityCode.Code;

        if (!ConstantComparison(lastCode, HashCode(model.VerificationCode)))
            return BadRequest(new ApiResponse(400, "Invalid reset code."));

        if (!identityCode.IsActive || identityCode.CreationTime.Minute + 5 < DateTime.UtcNow.Minute)
            return BadRequest(new ApiResponse(400, "This code has expired."));

        identityCode.IsActive = false;
        _identityContext.IdentityCodes.Update(identityCode);
        user.EmailConfirmed = true;
        await _userManager.UpdateAsync(user);
        await _identityContext.SaveChangesAsync();

        return Ok(new ApiResponse(200, "Email verified successfully."));
    }

    [HttpPost("login")]
	[ProducesResponseType(typeof(AppUserResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<AppUserResponse>> Login(LoginRequest model)
	{
		var user = await _userManager.FindByEmailAsync(model.Email);

		if (user is null || model.Password is null)
			return BadRequest(new ApiResponse(400, "Invalid email or password."));

		var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

		if (result.Succeeded is false)
			return BadRequest(new ApiResponse(400, "Invalid email or password."));

        var token = await _authService.CreateTokenAsync(user, _userManager);

        return Ok(new AppUserResponse
		{
			DisplayName = user.DisplayName,
			Email = model.Email,
			Token = token
		});
	}
    [HttpPost("SendPasswordResetEmail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SendPasswordResetEmail(EmailRequest emailDto)
    {
        
        if (await _userManager.FindByEmailAsync(emailDto.Email) is not { } user)
            return Ok(new ApiResponse(200, "If your email is registered with us, a password reset email has been successfully sent."));

        var code = GenerateSecureCode();

        EmailResponse emailToSend = new()
        {
            To = emailDto.Email,
            Subject = $"{user.DisplayName}, Reset Your Password - Verification Code: {code}",
            Body = EmailBody(code, user.DisplayName, "Reset Password", "You have requested to reset your password.")
        };

        try
        {
            await _identityContext.IdentityCodes.AddAsync(new IdentityCode()
            {
                Code = HashCode(code),
                User = user,
                AppUserId = user.Id,
                ForRegisterationConfirmed = false,
            });

            await _identityContext.SaveChangesAsync();

            await _emailSettings.SendEmailMessage(emailToSend);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while sending password reset email.");
            return StatusCode(500, new ApiResponse(500, "An error occurred while processing your request."));
        }

        return Ok(new ApiResponse(200, "If your email is registered with us, a password reset email has been successfully sent."));
    }

    [HttpPost("VerifyResetCode")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> VerifyResetCode(CodeVerificationRequest model)
    {
        if (await _userManager.FindByEmailAsync(model.Email) is not { } user)
            return BadRequest(new ApiResponse(400, "Invalid Email."));

        if (!user.EmailConfirmed)
            Ok(new ApiResponse(200, "You need to confirm your email first."));

        var identityCode = await _identityContext.IdentityCodes
                            .Where(P => P.AppUserId == user.Id && P.ForRegisterationConfirmed == false)
                            .OrderBy(d => d.CreationTime)
                            .LastOrDefaultAsync();

        if (identityCode is null)
            return BadRequest(new ApiResponse(400, "No valid reset code found."));

        if (identityCode.IsActive)
            return BadRequest(new ApiResponse(400, "You already have an active code."));

        var lastCode = identityCode.Code;

        if (!ConstantComparison(lastCode, HashCode(model.VerificationCode)))
            return BadRequest(new ApiResponse(400, "Invalid reset code."));

        if (identityCode.CreationTime.Minute + 5 < DateTime.UtcNow.Minute)
            return BadRequest(new ApiResponse(400, "This code has expired."));

        identityCode.IsActive = true;
        identityCode.ActivationTime = DateTime.UtcNow;
        _identityContext.IdentityCodes.Update(identityCode);
        await _identityContext.SaveChangesAsync();

        return Ok(new ApiResponse(200, "Code verified successfully."));
    }

    [HttpPost("changepassword")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ChangePassword(ChangePasswordRequest model)
    {
        if (await _userManager.FindByEmailAsync(model.Email) is not { } user)
            return BadRequest(new ApiResponse(400, "Invalid Email."));

        if (!user.EmailConfirmed)
            Ok(new ApiResponse(200, "You need to confirm your email first."));

        var identityCode = await _identityContext.IdentityCodes
                            .Where(p => p.AppUserId == user.Id && p.IsActive && p.ForRegisterationConfirmed == false)
                            .OrderByDescending(p => p.CreationTime)
                            .FirstOrDefaultAsync();

        if (identityCode is null)
            return BadRequest(new ApiResponse(400, "No valid reset code found."));

        var lastCode = identityCode.Code;

        if (!ConstantComparison(lastCode, HashCode(model.VerificationCode)))
            return BadRequest(new ApiResponse(400, "Invalid reset code."));


        if (identityCode is null)
            return BadRequest(new ApiResponse(400, "No valid reset code found."));

        if (identityCode.ActivationTime is null || identityCode.ActivationTime.Value.AddMinutes(30) < DateTime.UtcNow)
            return BadRequest(new ApiResponse(400, "This code has expired."));

        using var transaction = await _identityContext.Database.BeginTransactionAsync();

        try
        {
            identityCode.IsActive = false;
            _identityContext.IdentityCodes.Update(identityCode);
            await _identityContext.SaveChangesAsync();

            var removePasswordResult = await _userManager.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return BadRequest(new ApiResponse(400, "Failed to remove the old password."));
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return BadRequest(new ApiResponse(400, "Failed to set the new password."));
            }

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            _logger.LogError(ex, "Error occurred while changing password.");
            return StatusCode(500, new ApiResponse(500, "An error occurred while processing your request."));
        }

        return Ok(new ApiResponse(200, "Password changed successfully."));
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<UserAddressResponse>> GetCurrentUser()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);

        var user = await _userManager.FindByEmailAsync(email!);

        return Ok(new AppUserResponse()
        {
            DisplayName = user!.DisplayName,
            Email = user.Email!,
            Token = await _authService.CreateTokenAsync(user, _userManager)
        });
    }

    [Authorize]
    [HttpGet("address")]
    public async Task<ActionResult<UserAddressResponse>> GetCurrentUserAddress()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);

        var user = await _userManager.Users.Include(x => x.Address).SingleOrDefaultAsync(u => u.Email == email);

        return Ok(_mapper.Map<UserAddress, UserAddressResponse>(user.Address));
    }

    [Authorize]
    [HttpPut("address")]
    [ProducesResponseType(typeof(UserAddressResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserAddressResponse>> UpdateUserAddress(UserAddressResponse updatedAddress)
    {
        var address = _mapper.Map<UserAddressResponse, UserAddress>(updatedAddress);

        var userEmail = User.FindFirstValue(ClaimTypes.Email);

        var user = await _userManager.Users.Include(x => x.Address).SingleOrDefaultAsync(u => u.Email == userEmail);

        user!.Address = address;

        address.AppUserId = user.Id;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return BadRequest(new ApiResponse(400));

        return Ok(updatedAddress);
    }

    [HttpPost("googlelogin")]
    [ProducesResponseType(typeof(AppUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GoogleLogin(TokenIdRequest tokenIdDto)
    {
        if (ValidateGoogleToken(tokenIdDto.TokenId, out JObject payload))
        {
            var userName = payload["name"]!.ToString();
            var email = payload["email"]!.ToString();

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                user = new AppUser
                {
                    UserName = email.Split('@')[0],
                    Email = email,
                    DisplayName = userName,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user);

                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponse(400, "Failed to create user."));
                }
            }

            user.EmailConfirmed = true;

            await _userManager.UpdateAsync(user);

            var token = await _authService.CreateTokenAsync(user, _userManager);

            return Ok(new AppUserResponse
            {
                DisplayName = user.DisplayName,
                Email = user!.Email,
                Token = token
            });
        }
        else
        {
            return BadRequest(new ApiResponse(400, "Invalid Google token."));
        }
    }

    private bool ValidateGoogleToken(string tokenId, out JObject payload)
    {
        var httpClient = new HttpClient();
        var response = httpClient.GetAsync($"https://www.googleapis.com/oauth2/v1/userinfo?access_token={tokenId}").Result;
        if (response.IsSuccessStatusCode)
        {
            var json = response.Content.ReadAsStringAsync().Result;
            payload = JObject.Parse(json);
            return true;
        }
        payload = null;
        return false;
    }

    private string EmailBody(string code, string userName, string title, string message)
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
                            <p>&copy; 2024 TwoAxis. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";
    }

    private string GenerateSecureCode()
    {
        byte[] randomNumber = new byte[4];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        int result = BitConverter.ToInt32(randomNumber, 0);
        int positiveResult = Math.Abs(result);

        int sixDigitCode = positiveResult % 1000000;
        return sixDigitCode.ToString("D6");
    }

    private string HashCode(string code)
    {
        var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(code));
        return BitConverter.ToString(hashedBytes).Replace("-", "");
    }

    private async Task<bool> CheckEmailExist(string email)
	{
		return await _userManager.FindByEmailAsync(email) is not null;
	}

    private bool ConstantComparison(string a, string b)
    {
        if (a.Length != b.Length)
            return false;

        int result = 0;
        for (int i = 0; i < a.Length; i++)
        {
            result |= a[i] ^ b[i];
        }
        return result == 0;
    }
}