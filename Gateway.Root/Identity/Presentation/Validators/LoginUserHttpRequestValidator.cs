using FluentValidation;
using Gateway.Root.Identity.Presentation.Models;

namespace Gateway.Root.Identity.Presentation.Validators;

public class LoginUserHttpRequestValidator : AbstractValidator<LoginUserHttpRequest>
{
	public LoginUserHttpRequestValidator()
	{
		RuleFor(user => user.Email).NotNull().EmailAddress();
		RuleFor(user => user.Password).NotEmpty();
	}
}