using Core.Entities.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Shared.Dtos;
using Shared.Helpers;
using System.Security.Claims;

namespace Core.Interfaces.Services;
public interface IAuthService
{
    Task<string> CreateTokenAsync(AppUser user, UserManager<AppUser> userManager);

    Task<Result> SendEmailVerificationCode(ClaimsPrincipal User);

    Task<Result> VerifyRegisterCode(CodeVerificationRequest model, ClaimsPrincipal User);

    Task<Result> SendPasswordResetEmail(EmailRequest email);

    Task<Result> VerifyResetCode(CodeVerificationRequest model, ClaimsPrincipal User);

    Task<Result> ChangePassword(ChangePasswordRequest model, ClaimsPrincipal User);
}