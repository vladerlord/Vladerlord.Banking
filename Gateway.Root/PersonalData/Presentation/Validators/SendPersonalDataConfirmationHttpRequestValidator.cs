using FluentValidation;
using Gateway.Root.PersonalData.Presentation.Models;

namespace Gateway.Root.PersonalData.Presentation.Validators;

public class SendPersonalDataConfirmationHttpRequestValidator : AbstractValidator<SendPersonalDataConfirmationRequest>
{
	public SendPersonalDataConfirmationHttpRequestValidator()
	{
		RuleFor(personalData => personalData.FirstName).NotEmpty();
		RuleFor(personalData => personalData.LastName).NotEmpty();
		RuleFor(personalData => personalData.Country).NotEmpty();
		RuleFor(personalData => personalData.City).NotEmpty();
		RuleFor(personalData => personalData.Files).NotNull();
	}
}