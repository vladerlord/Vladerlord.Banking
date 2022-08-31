using FluentValidation;
using Gateway.Root.BankAccount.Presentation.HttpModel;

namespace Gateway.Root.BankAccount.Presentation.Validator;

public class WithdrawValidator : AbstractValidator<WithdrawRequest>
{
    public WithdrawValidator()
    {
        RuleFor(request => request.BankAccountId).Must(value => Guid.TryParse(value, out _))
            .WithMessage("Must be a proper GUID");
        RuleFor(request => request.Currency).NotEmpty();
        RuleFor(request => request.Amount).NotEmpty();
    }
}
