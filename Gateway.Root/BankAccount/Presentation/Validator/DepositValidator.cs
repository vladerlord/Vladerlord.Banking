using FluentValidation;
using Gateway.Root.BankAccount.Presentation.HttpModel;

namespace Gateway.Root.BankAccount.Presentation.Validator;

public class DepositValidator : AbstractValidator<DepositRequest>
{
    public DepositValidator()
    {
        RuleFor(request => request.BankAccountId).Must(value => Guid.TryParse(value, out _))
            .WithMessage("Must be a proper GUID");
        RuleFor(request => request.Currency).NotEmpty();
        RuleFor(request => request.Amount).NotEmpty();
    }
}
