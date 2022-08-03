using FluentValidation;
using Gateway.Root.Identity.Presentation.Models;

namespace Gateway.Root.Identity.Presentation.Validators;

public class RegisterUserHttpRequestValidator : AbstractValidator<RegisterUserHttpRequest>
{
    public RegisterUserHttpRequestValidator()
    {
        RuleFor(user => user.Email).NotNull().EmailAddress();
        RuleFor(user => user.Password).NotEmpty();
    }
}