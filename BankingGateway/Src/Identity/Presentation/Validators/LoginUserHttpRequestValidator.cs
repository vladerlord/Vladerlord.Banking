using BankingGateway.Identity.Presentation.Models;
using FluentValidation;

namespace BankingGateway.Identity.Presentation.Validators;

public class LoginUserHttpRequestValidator : AbstractValidator<LoginUserHttpRequest>
{
    public LoginUserHttpRequestValidator()
    {
        RuleFor(user => user.Email).NotNull().EmailAddress();
        RuleFor(user => user.Password).NotEmpty();
    }
}