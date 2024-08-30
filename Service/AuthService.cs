using Core.Dtos;
using Core.Entities.IdentityEntities;
using Core.ErrorHandling;
using Core.Interfaces.Services;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Repository.Identity;
using Service.ConfigurationData;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Service;
public class AuthService(IOptions<JWTData> jWTData, UserManager<AppUser> _userManager, IdentityContext _identityContext, IEmailSettingService _emailSettings) : IAuthService
{
    private readonly JWTData _jWTData = jWTData.Value;

    public async Task<string> CreateTokenAsync(AppUser user, UserManager<AppUser> userManager)
    {

        // Private Claims (user defined - can change from user to other)
        var authClaims = new List<Claim>()
        {
            new (ClaimTypes.GivenName, user.UserName!),
            new (ClaimTypes.Email, user.Email!)
        };

        var userRoles = await userManager.GetRolesAsync(user);

        foreach (var role in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        // secret key
        var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jWTData.SecretKey));

        // Token Object
        var token = new JwtSecurityToken(
            // Registered Claims
            issuer: _jWTData.ValidIssuer,
            audience: _jWTData.ValidAudience,
            expires: DateTime.UtcNow.AddMinutes(_jWTData.DurationInMinutes),
            // Private Claims
            claims: authClaims,
            // Signature Key
            signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature)
        );

        // Create Token And Return It
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<Result> SendEmailVerificationCode(ClaimsPrincipal User)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);

        if (await _userManager.FindByEmailAsync(userEmail!) is not { } user)
            return Result.Success("If your email is registered with us, a email verification code has been successfully sent.");

        if (user.EmailConfirmed)
            return Result.Failure(400, "Your email is already confirmed.");

        var code = GenerateSecureCode();

        EmailResponse emailToSend = new()
        {
            To = userEmail!,
            Subject = $"{userEmail!.Split('@')[0]}, Your pin code is {code}. \r\nPlease confirm your email address",
            Body = EmailBody(code, userEmail.Split('@')[0], "Email Verification", "Thank you for registering with our service. To complete your registration")
        };

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

        return Result.Success("If your email is registered with us, a email verification code has been successfully sent.");
    }

    public async Task<Result> VerifyRegisterCode(CodeVerificationRequest model, ClaimsPrincipal User)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);

        if (await _userManager.FindByEmailAsync(userEmail!) is not { } user)
            return Result.Failure(400, "No account found with the provided email address.");

        var identityCode = await _identityContext.IdentityCodes
                            .Where(P => P.AppUserId == user.Id && P.ForRegisterationConfirmed)
                            .OrderBy(d => d.CreationTime)
                            .LastOrDefaultAsync();

        if (identityCode is null)
            return Result.Failure(400, "The reset code is missing or invalid. Please request a new reset code.");

        var lastCode = identityCode.Code;

        if (!ConstantComparison(lastCode, HashCode(model.VerificationCode)))
            return Result.Failure(400, "The reset code is missing or invalid. Please request a new reset code.");

        if (!identityCode.IsActive || identityCode.CreationTime.Minute + 5 < DateTime.UtcNow.Minute)
            return Result.Failure(400, "The reset code has either expired or is not active. Please request a new code.");

        identityCode.IsActive = false;

        _identityContext.IdentityCodes.Update(identityCode);

        user.EmailConfirmed = true;

        await _userManager.UpdateAsync(user);

        await _identityContext.SaveChangesAsync();

        return Result.Success("Your email has been successfully verified. You can now proceed with your account activities.");
    }

    public async Task<Result> SendPasswordResetEmail(EmailRequest email)
    {
        if (await _userManager.FindByEmailAsync(email.Email) is not { } user)
            return Result.Success("If your email is registered with us, a password reset email has been successfully sent.");

        var code = GenerateSecureCode();

        EmailResponse emailToSend = new()
        {
            To = email.Email,
            Subject = $"{user.DisplayName}, Reset Your Password - Verification Code: {code}",
            Body = EmailBody(code, user.DisplayName, "Reset Password", "You have requested to reset your password.")
        };

        await _identityContext.IdentityCodes.AddAsync(new IdentityCode()
        {
            Code = HashCode(code),
            User = user,
            AppUserId = user.Id,
            ForRegisterationConfirmed = false,
        });

        await _identityContext.SaveChangesAsync();

        BackgroundJob.Enqueue(() => _emailSettings.SendEmailMessage(emailToSend));

        return Result.Success("If your email is registered with us, a email verification code has been successfully sent.");
    }

    public async Task<Result> VerifyResetCode(CodeVerificationRequest model, ClaimsPrincipal User)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);

        if (await _userManager.FindByEmailAsync(userEmail!) is not { } user)
            return Result.Failure(400, "No account associated with the provided email address was found, Please check the email and try again.");

        if (user.EmailConfirmed is false)
            return Result.Failure(400, "Please verify your email address before proceeding.");

        var identityCode = await _identityContext.IdentityCodes
                            .Where(P => P.AppUserId == user.Id && P.ForRegisterationConfirmed == false)
                            .OrderBy(d => d.CreationTime)
                            .LastOrDefaultAsync();

        if (identityCode is null)
            return Result.Failure(400, "The reset code is missing or invalid. Please request a new reset code.");

        if (identityCode.IsActive)
            return Result.Failure(400, "An active reset code already exists. Please use the existing code or wait until it expires to request a new one.");

        var lastCode = identityCode.Code;

        if (!ConstantComparison(lastCode, HashCode(model.VerificationCode)))
            return Result.Failure(400, "The reset code is missing or invalid. Please request a new reset code.");

        if (identityCode.CreationTime.Minute + 5 < DateTime.UtcNow.Minute)
            return Result.Failure(400, "The reset code has either expired or is not active. Please request a new code.");

        user.EmailConfirmed = true;

        identityCode.IsActive = true;

        identityCode.User = user;

        identityCode.ActivationTime = DateTime.UtcNow;

        _identityContext.IdentityCodes.Update(identityCode);

        await _userManager.UpdateAsync(user);

        await _identityContext.SaveChangesAsync();

        return Result.Success("Code verified successfully. You have 30 minutes from now to reset your password.");
    }

    public async Task<Result> ChangePassword(ChangePasswordRequest model, ClaimsPrincipal User)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);

        if (await _userManager.FindByEmailAsync(userEmail!) is not { } user)
            return Result.Failure(400, "No account associated with the provided email address was found, Please check the email and try again.");

        if (user.EmailConfirmed is false) 
            return Result.Failure(400, "Please verify your email address before proceeding.");

        var identityCode = await _identityContext.IdentityCodes
                            .Where(p => p.AppUserId == user.Id && p.IsActive && p.ForRegisterationConfirmed == false)
                            .OrderByDescending(p => p.CreationTime)
                            .FirstOrDefaultAsync();

        if (identityCode is null)
            return Result.Failure(400, "The reset code is missing or invalid. Please request a new reset code.");

        var lastCode = identityCode.Code;

        if (!ConstantComparison(lastCode, HashCode(model.VerificationCode)))
            return Result.Failure(400, "The reset code is missing or invalid. Please request a new reset code.");

        if (identityCode.ActivationTime is null || identityCode.ActivationTime.Value.AddMinutes(30) < DateTime.UtcNow)
            return Result.Failure(400, "The reset code has either expired or is not active. Please request a new code.");

        using var transaction = await _identityContext.Database.BeginTransactionAsync();

        identityCode.IsActive = false;

        _identityContext.IdentityCodes.Update(identityCode);

        await _identityContext.SaveChangesAsync();

        var removePasswordResult = await _userManager.RemovePasswordAsync(user);

        if (removePasswordResult.Succeeded is false)
        {
            await transaction.RollbackAsync();

            var errors = string.Join(", ", removePasswordResult.Errors.Select(e => e.Description));

            return Result.Failure(400, errors);
        }

        var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);

        if (!addPasswordResult.Succeeded)
        {
            await transaction.RollbackAsync();

            var errors = string.Join(", ", addPasswordResult.Errors.Select(e => e.Description));

            return Result.Failure(400, errors);
        }

        await transaction.CommitAsync();

        return Result.Success("Your password has been successfully changed, You can now log in with your new credentials.");
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