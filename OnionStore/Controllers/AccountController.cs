using Shared.Dtos;
using API.Errors;
using Core.Entities.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using API.EmailSetting;
using Repository.Identity;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AutoMapper;
using Google.Apis.Auth;
using Hangfire;
using API.Extensions;

namespace API.Controllers;
public class AccountController(IAccountService _accountService, UserManager<AppUser> _userManager, SignInManager<AppUser> _signInManager, IMapper _mapper,
             IdentityContext _identityContext, IEmailSettings _emailSettings, ILogger<AccountController> _logger, IAuthService _authService) : BaseController
{

	[HttpPost("register")]
	public async Task<ActionResult> Register(RegisterRequest model)
	{
        var result = await _accountService.Register(model);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
	}

    [Authorize]
    [HttpPost("send-email-verification-code")]
    public async Task<ActionResult<AppUserResponse>> SendEmailVerificationCode()
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);

        if (await _userManager.FindByEmailAsync(userEmail!) is not { } user)
            return Ok(new ApiResponse(200, "If your email is registered with us, a email verification code has been successfully sent."));

        if (user.EmailConfirmed)
            return Ok(new ApiResponse(200, "Your email is already confirmed."));

        var code = GenerateSecureCode();

        EmailResponse emailToSend = new()
        {
            To = userEmail!,
            Subject = $"{userEmail!.Split('@')[0]}, Your pin code is {code}. \r\nPlease confirm your email address",
            Body = EmailBody(code, userEmail.Split('@')[0], "Email Verification", "Thank you for registering with our service. To complete your registration")
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

            BackgroundJob.Enqueue(() => _emailSettings.SendEmailMessage(emailToSend));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while sending password reset email.");
            return StatusCode(500, new ApiResponse(500, "An error occurred while processing your request."));
        }

        return Ok(new ApiResponse(200, "If your email is registered with us, a email verification code has been successfully sent."));
    }

    [Authorize]
    [HttpPost("verify-register-code")]
    public async Task<ActionResult> VerifyRegisterCode(CodeVerificationRequest model)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);

        if(await _userManager.FindByEmailAsync(userEmail!) is not { } user)
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
	public async Task<ActionResult<AppUserResponse>> Login(LoginRequest model)
	{
		var result = await _accountService.Login(model);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
	}

    [HttpPost("send-password-verification-code")]
    public async Task<ActionResult> SendPasswordResetEmail(EmailRequest email)
    {
        if (await _userManager.FindByEmailAsync(email.Email) is not { } user)
            return Ok(new ApiResponse(200, "If your email is registered with us, a password reset email has been successfully sent."));

        var code = GenerateSecureCode();

        EmailResponse emailToSend = new()
        {
            To = email.Email,
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

            BackgroundJob.Enqueue(() => _emailSettings.SendEmailMessage(emailToSend));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while sending password reset email.");
            return StatusCode(500, new ApiResponse(500, "An error occurred while processing your request."));
        }

        return Ok(new ApiResponse(200, "If your email is registered with us, a password reset email has been successfully sent."));
    }

    [HttpPost("Verify-Reset-Code")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> VerifyResetCode(CodeVerificationRequest model)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);

        if (await _userManager.FindByEmailAsync(userEmail!) is not { } user)
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

        user.EmailConfirmed = true;

        identityCode.IsActive = true;

        identityCode.User = user;

        identityCode.ActivationTime = DateTime.UtcNow;

        _identityContext.IdentityCodes.Update(identityCode);

        await _userManager.UpdateAsync(user);

        await _identityContext.SaveChangesAsync();

        return Ok(new ApiResponse(200, "Code verified successfully. You have 30 minutes to change your password."));
    }

    [Authorize]
    [HttpPost("changepassword")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ChangePassword(ChangePasswordRequest model)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);

        if (await _userManager.FindByEmailAsync(userEmail!) is not { } user)
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
    [ProducesResponseType(typeof(AppUserResponse), StatusCodes.Status200OK)]
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
        var result = await _accountService.GetCurrentUserAddress(User);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [Authorize]
    [HttpPut("address")]
    public async Task<ActionResult<UserAddressResponse>> UpdateUserAddress(UserAddressResponse updatedAddress)
    {
        var result = await _accountService.UpdateUserAddress(updatedAddress, User);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("googlelogin")]
    [ProducesResponseType(typeof(AppUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AppUserResponse>> GoogleLogin([FromBody] string credential)
    {
        var result = await _accountService.GoogleLogin(credential);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
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
                            <p>&copy; 2024 OnionStore. All rights reserved.</p>
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