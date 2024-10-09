using BlazorEcommerce.Application.Dtos;
using FluentValidation;

namespace BlazorEcommerce.Application.Validations;
public class OrderAddressValidator: AbstractValidator<OrderAddressRequest>
{
    public OrderAddressValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(50)
            .WithMessage("First name must not exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(50)
            .WithMessage("Last name must not exceed 50 characters");

        RuleFor(x => x.Street)
            .NotEmpty()
            .WithMessage("Street is required")
            .MaximumLength(50)
            .WithMessage("Street must not exceed 50 characters");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City is required")
            .MaximumLength(50)
            .WithMessage("City must not exceed 50 characters");

        RuleFor(x => x.Country)
            .NotEmpty()
            .WithMessage("Country is required")
            .MaximumLength(50)
            .WithMessage("Country must not exceed 50 characters");
    }
}