//using BlazorEcommerce.Shared.Account;
//using FluentValidation;

//namespace BlazorEcommerce.Shared.AccountValidation;
//public class LoginRequestValidator: AbstractValidator<LoginRequest>
//{
//	public LoginRequestValidator()
//	{
//		RuleFor(x => x.Email)
//			.NotEmpty()
//			.WithMessage("Email is required")
//			.EmailAddress()
//			.WithMessage("Email is not valid");

//		RuleFor(x => x.Password)
//			.NotEmpty()
//			.WithMessage("Password is required")
//			.MinimumLength(6)
//			.WithMessage("Password must be at least 6 characters");
//	}
//}