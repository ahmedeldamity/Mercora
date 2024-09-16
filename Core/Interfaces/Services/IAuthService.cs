using Core.Dtos;
using Core.ErrorHandling;
using System.Security.Claims;

namespace Core.Interfaces.Services;
public interface IAuthService
{
    Task<Result> SendEmailVerificationCode(ClaimsPrincipal userClaims);
    Task<Result> SendEmailVerificationCodeV2(ClaimsPrincipal userClaims);
    Task<Result> VerifyRegisterCode(CodeVerificationRequest model, ClaimsPrincipal userClaims);
    Task<Result> SendPasswordResetEmail(EmailRequest email);
    Task<Result> SendPasswordResetEmailV2(EmailRequest email);
    Task<Result> VerifyResetCode(CodeVerificationRequest model, ClaimsPrincipal userClaims);
    Task<Result> ChangePassword(ChangePasswordRequest model, ClaimsPrincipal userClaims);
}