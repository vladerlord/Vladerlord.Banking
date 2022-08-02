using BankingGateway.Identity.Presentation.Models;
using FluentValidation;

namespace BankingGateway.Identity.Presentation.Validators;

public class RegisterUserHttpRequestValidator : AbstractValidator<RegisterUserHttpRequest>
{
    public RegisterUserHttpRequestValidator()
    {
        RuleFor(user => user.Email).NotNull().EmailAddress();
        RuleFor(user => user.Password).NotEmpty();
    }
}