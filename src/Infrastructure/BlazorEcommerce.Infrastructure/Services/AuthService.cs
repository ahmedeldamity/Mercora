using BlazorEcommerce.Application.Dtos;
using BlazorEcommerce.Application.Interfaces.Repositories;
using BlazorEcommerce.Application.Interfaces.Services;
using BlazorEcommerce.Application.Models;
using BlazorEcommerce.Application.Specifications.Identity;
using BlazorEcommerce.Domain.Entities.IdentityEntities;
using BlazorEcommerce.Domain.ErrorHandling;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BlazorEcommerce.Infrastructure.Services;
public class AuthService(IOptions<JwtData> jWtData, UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IAccountService accountService,
IEmailSettingService emailSettings) : IAuthService
{
    private readonly JwtData _jWtData = jWtData.Value;

    public async Task<Result> SendEmailVerificationCode(string userEmail)
    {
		var user = await userManager.FindByEmailAsync(userEmail);

		if (user is not null)
			return Result.Failure<AppUserResponse>(new Error(400, "The email address you entered is already taken, Please try a different one."));

		var code = GenerateSecureCode();

		var subject = $"✅ {userEmail!.Split('@')[0]}, Your pin code is {code}. \r\nPlease confirm your email address";

		var body = EmailBody(code, userEmail.Split('@')[0], "Email Verification", "Thank you for registering with our service. To complete your registration");

		EmailResponse emailToSend = new(subject, body, userEmail);

        var identityCode = new IdentityCode
		{
			Code = HashCode(code),
			IsActive = true,
			Email = userEmail,
			ForRegistrationConfirmed = true
		};

		await unitOfWork.Repository<IdentityCode>().AddAsync(identityCode);

		await unitOfWork.CompleteAsync();

		BackgroundJob.Enqueue(() => emailSettings.SendEmailMessage(emailToSend));

		return Result.Success();
	}

    public async Task<Result> SendEmailVerificationCodeV2(string userEmail)
    {
		var user = await userManager.FindByEmailAsync(userEmail);

		if (user is not null)
			return Result.Failure<AppUserResponse>(new Error(400, "The email address you entered is already taken, Please try a different one."));

		var code = GenerateSecureCode();

        var subject = $"✅ {userEmail!.Split('@')[0]}, Your pin code is {code}. \r\nPlease confirm your email address";

        var body = LoadEmailTemplate("Templates/EmailTemplate.html", code, user.DisplayName, "Reset Password", "You have requested to reset your password.");

        EmailResponse emailToSend = new(subject, body, userEmail);

        await unitOfWork.Repository<IdentityCode>().AddAsync(new IdentityCode
        {
	        Code = HashCode(code),
	        IsActive = true,
			Email = userEmail,
			ForRegistrationConfirmed = true
        });

		await unitOfWork.CompleteAsync();

        BackgroundJob.Enqueue(() => emailSettings.SendEmailMessage(emailToSend));

        return Result.Success("If your email is registered with us, a email verification code has been successfully sent.");
    }

	public async Task<Result<AppUserResponse>> VerifyRegisterCode(CodeVerificationRequest model)
    {
		var user = await userManager.FindByEmailAsync(model.Email);

		if (user is not null)
			return Result.Failure<AppUserResponse>(new Error(400, "The email address you entered is already taken, Please try a different one."));

		var spec = new IdentityCodeSpecification(model.Email);

		var identityCodes = await unitOfWork.Repository<IdentityCode>().GetAllAsync(spec);

		var identityCode = identityCodes.LastOrDefault();
	    
		if (identityCode is null)
			return Result.Failure<AppUserResponse>(new Error(400, "The reset code is missing or invalid. Please request a new reset code."));

		var lastCode = identityCode.Code;

		if (!ConstantComparison(lastCode, HashCode(model.VerificationCode)))
			return Result.Failure<AppUserResponse>(new Error(400, "The reset code is missing or invalid. Please request a new reset code."));

		if (!identityCode.IsActive || identityCode.CreationTime.Minute + 5 < DateTime.UtcNow.Minute)
			return Result.Failure<AppUserResponse>(new Error(400, "The reset code has either expired or is not active. Please request a new code."));

		identityCode.IsActive = false;

		unitOfWork.Repository<IdentityCode>().Update(identityCode);

		await unitOfWork.CompleteAsync();

        var registerRequest = new RegisterRequest(model.DisplayName, model.Email);

		var result = await accountService.Register(registerRequest);

		return result;
	}

    public async Task<Result> SendPasswordResetEmail(EmailRequest email)
    {
        if (await userManager.FindByEmailAsync(email.Email) is not { } user)
            return Result.Success();

        var code = GenerateSecureCode();

        var subject = $"✅ {user.DisplayName}, Reset Your Password - Verification Code: {code}";

        var body = EmailBody(code, user.DisplayName, "Reset Password", "You have requested to reset your password.");

        EmailResponse emailToSend = new(subject, body, email.Email);

        await unitOfWork.Repository<IdentityCode>().AddAsync(new IdentityCode
        {
            Code = HashCode(code),
            Email = email.Email,
            ForRegistrationConfirmed = false,
        });
	    
        await unitOfWork.CompleteAsync();

        BackgroundJob.Enqueue(() => emailSettings.SendEmailMessage(emailToSend));

        return Result.Success();
    }

    public async Task<Result> SendPasswordResetEmailV2(EmailRequest email)
    {
        if (await userManager.FindByEmailAsync(email.Email) is not { } user)
            return Result.Success("If your email is registered with us, a password reset email has been successfully sent.");

        var code = GenerateSecureCode();

        var subject = $"✅ {user.DisplayName}, Reset Your Password - Verification Code: {code}";

        var body = LoadEmailTemplate("Templates/EmailTemplate.html", code, user.DisplayName, "Reset Password", "You have requested to reset your password.");

        EmailResponse emailToSend = new(subject, body, email.Email);

        await unitOfWork.Repository<IdentityCode>().AddAsync(new IdentityCode
        {
            Code = HashCode(code),
            Email = user.Email,
            ForRegistrationConfirmed = false
        });

        await unitOfWork.CompleteAsync();

        BackgroundJob.Enqueue(() => emailSettings.SendEmailMessage(emailToSend));

        return Result.Success("If your email is registered with us, a password reset email has been successfully sent.");
    }

    public async Task<Result> VerifyResetCode(CodeVerificationRequest model, ClaimsPrincipal userClaims)
    {
        var userEmail = userClaims.FindFirstValue(ClaimTypes.Email);

        if (await userManager.FindByEmailAsync(userEmail!) is not { } user)
            return Result.Failure(new Error(400, "No account associated with the provided email address was found, Please check the email and try again."));

        if (user.EmailConfirmed is false)
            return Result.Failure(new Error(400, "Please verify your email address before proceeding."));

        var spec = new IdentityCodeSpecification(user.Id, false);

        var identityCodes = await unitOfWork.Repository<IdentityCode>().GetAllAsync(spec);

        var identityCode = identityCodes.LastOrDefault();

        if (identityCode is null)
            return Result.Failure(new Error(400, "The reset code is missing or invalid. Please request a new reset code."));

        if (identityCode.IsActive)
            return Result.Failure(new Error(400, "An active reset code already exists. Please use the existing code or wait until it expires to request a new one."));

        var lastCode = identityCode.Code;

        if (!ConstantComparison(lastCode, HashCode(model.VerificationCode)))
            return Result.Failure(new Error(400, "The reset code is missing or invalid. Please request a new reset code."));

        if (identityCode.CreationTime.Minute + 5 < DateTime.UtcNow.Minute)
            return Result.Failure(new Error(400, "The reset code has either expired or is not active. Please request a new code."));

        user.EmailConfirmed = true;

        identityCode.IsActive = true;

        identityCode.ActivationTime = DateTime.UtcNow;

        unitOfWork.Repository<IdentityCode>().Update(identityCode);

        await userManager.UpdateAsync(user);

        await unitOfWork.CompleteAsync();

        return Result.Success();
    }

    public async Task<Result> ChangePassword(ChangePasswordRequest model, ClaimsPrincipal userClaims)
    {
        var userEmail = userClaims.FindFirstValue(ClaimTypes.Email);

        if (await userManager.FindByEmailAsync(userEmail!) is not { } user)
            return Result.Failure(new Error(400, "No account associated with the provided email address was found, Please check the email and try again."));

        if (user.EmailConfirmed is false)
            return Result.Failure(new Error(400, "Please verify your email address before proceeding."));

        var spec = new IdentityCodeSpecification(user.Id, false);

        var identityCodes = await unitOfWork.Repository<IdentityCode>().GetAllAsync(spec);

        var identityCode = identityCodes.LastOrDefault();

        if (identityCode is null)
            return Result.Failure(new Error(400, "The reset code is missing or invalid. Please request a new reset code."));

        var lastCode = identityCode.Code;

        if (!ConstantComparison(lastCode, HashCode(model.VerificationCode)))
            return Result.Failure(new Error(400, "The reset code is missing or invalid. Please request a new reset code."));

        if (!identityCode.IsActive || identityCode.ActivationTime is null || identityCode.ActivationTime.Value.AddMinutes(30) < DateTime.UtcNow)
            return Result.Failure(new Error(400, "The reset code has either expired or is not active. Please request a new code."));

        using var transaction = unitOfWork.BeginTransaction();
	    
        identityCode.IsActive = false;

        unitOfWork.Repository<IdentityCode>().Update(identityCode);

        await unitOfWork.CompleteAsync();

        var removePasswordResult = await userManager.RemovePasswordAsync(user);

        if (removePasswordResult.Succeeded is false)
        {
            await transaction.RollbackTransactionAsync();

            var errors = string.Join(", ", removePasswordResult.Errors.Select(e => e.Description));

            return Result.Failure(new Error(400, errors));
        }

        var addPasswordResult = await userManager.AddPasswordAsync(user, model.NewPassword);

        if (!addPasswordResult.Succeeded)
        {
            await transaction.RollbackTransactionAsync();

            var errors = string.Join(", ", addPasswordResult.Errors.Select(e => e.Description));

            return Result.Failure(new Error(400, errors));
        }

        await transaction.CommitTransactionAsync();

        return Result.Success();
    }

    private static string LoadEmailTemplate(string filePath, string code, string userName, string title, string message)
    {
        var template = File.ReadAllText(filePath);

        template = template.Replace("{{Code}}", code)
                           .Replace("{{UserName}}", userName)
                           .Replace("{{Title}}", title)
                           .Replace("{{Message}}", message)
                           .Replace("{{Year}}", DateTime.Now.Year.ToString());

        return template;
    }

    private static string EmailBody(string code, string userName, string title, string message)
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

    private static string GenerateSecureCode()
    {
        var randomNumber = new byte[4];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        var result = BitConverter.ToInt32(randomNumber, 0);
        var positiveResult = Math.Abs(result);

        var sixDigitCode = positiveResult % 1000000;
        return sixDigitCode.ToString("D6");
    }

    private static string HashCode(string code)
    {
        var sha256 = SHA256.Create();
        var hashedBytes = sha256?.ComputeHash(Encoding.UTF8.GetBytes(code));
        return BitConverter.ToString(hashedBytes!).Replace("-", "");
    }

    private static bool ConstantComparison(string a, string b)
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