using BlazorEcommerce.Domain.ErrorHandling;
using System.Security.Claims;
using BlazorEcommerce.Application.Dtos;

namespace BlazorEcommerce.Application.Interfaces.Services;
public interface IAuthService
{
	Task<Result> SendEmailVerificationCode(string userEmail);
	Task<Result> SendEmailVerificationCodeV2(string userEmail);
	Task<Result<AppUserResponse>> VerifyRegisterCode(CodeVerificationRequest model);
}