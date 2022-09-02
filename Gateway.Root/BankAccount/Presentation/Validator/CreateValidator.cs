using System.Globalization;
using FluentValidation;
using Gateway.Root.BankAccount.Presentation.HttpModel;

namespace Gateway.Root.BankAccount.Presentation.Validator;

public class CreateValidator : AbstractValidator<CreateRequest>
{
    public CreateValidator()
    {
        RuleFor(request => request.CurrencyCode).NotEmpty();
        RuleFor(request => request.ExpireAt).Must(date =>
            DateOnly.TryParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _));
    }
}
