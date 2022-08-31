using FluentValidation;
using Gateway.Root.BankAccount.Presentation.HttpModel;

namespace Gateway.Root.BankAccount.Presentation.Validator;

public class TransferValidator : AbstractValidator<TransferRequest>
{
    public TransferValidator()
    {
        RuleFor(request => request.FromBankAccountId).Must(value => Guid.TryParse(value, out _))
            .WithMessage("Must be a proper GUID");
        RuleFor(request => request.ToBankAccountId).Must(value => Guid.TryParse(value, out _))
            .WithMessage("Must be a proper GUID");
        RuleFor(request => request.Amount).NotEmpty();
    }
}
