using FluentValidation;
using Gateway.Root.BankAccount.Presentation.HttpModel;

namespace Gateway.Root.BankAccount.Presentation.Validator;

public class CreateValidator: AbstractValidator<CreateRequest>
{
    public CreateValidator()
    {
        RuleFor(request => request.CurrencyCode).NotEmpty();
        RuleFor(request => request.ExpireAt).Must(date => DateOnly.TryParse(date, out _));
    }
}
