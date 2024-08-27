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
using Newtonsoft.Json.Linq;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AutoMapper;
using Google.Apis.Auth;

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

    [Authorize]
    [HttpPost("sendemailverificationcode")]
    [ProducesResponseType(typeof(AppUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
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

            await _emailSettings.SendEmailMessage(emailToSend);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while sending password reset email.");
            return StatusCode(500, new ApiResponse(500, "An error occurred while processing your request."));
        }

        return Ok(new ApiResponse(200, "If your email is registered with us, a email verification code has been successfully sent."));
    }

    [Authorize]
    [ProducesResponseType(typeof(AppUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [HttpPost("verifyregistercode")]
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

    [Authorize]
    [HttpPost("SendPasswordResetEmail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SendPasswordResetEmail()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);

        if (await _userManager.FindByEmailAsync(email!) is not { } user)
            return Ok(new ApiResponse(200, "If your email is registered with us, a password reset email has been successfully sent."));

        var code = GenerateSecureCode();

        EmailResponse emailToSend = new()
        {
            To = email!,
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

    [Authorize]
    [HttpPost("VerifyResetCode")]
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
    [ProducesResponseType(typeof(UserAddressResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserAddressResponse>> GetCurrentUserAddress()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);

        var user = await _userManager.Users.Include(x => x.Address).SingleOrDefaultAsync(u => u.Email == email);

        if (user!.Address is null)
            return NotFound(new ApiResponse(404, "Address not found."));

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
    public async Task<ActionResult> GoogleLogin([FromBody] string credential)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = [ "YOUR_CLIENT" ]
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(credential, settings);

        var user = await _userManager.FindByEmailAsync(payload.Email);

        if (user is null)
        {
            user = new AppUser
            {
                UserName = payload.Email.Split('@')[0],
                Email = payload.Email,
                DisplayName = payload.Name,
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
            Email = user.Email!,
            Token = token
        });

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