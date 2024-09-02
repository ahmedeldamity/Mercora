using Core.Dtos;
using Core.ErrorHandling;
using System.Security.Claims;

namespace Core.Interfaces.Services;
public interface IAuthService
{
    Task<Result> SendEmailVerificationCode(ClaimsPrincipal User);
    Task<Result> SendEmailVerificationCodeV2(ClaimsPrincipal User);
    Task<Result> VerifyRegisterCode(CodeVerificationRequest model, ClaimsPrincipal User);
    Task<Result> SendPasswordResetEmail(EmailRequest email);
    Task<Result> VerifyResetCode(CodeVerificationRequest model, ClaimsPrincipal User);
    Task<Result> ChangePassword(ChangePasswordRequest model, ClaimsPrincipal User);
}