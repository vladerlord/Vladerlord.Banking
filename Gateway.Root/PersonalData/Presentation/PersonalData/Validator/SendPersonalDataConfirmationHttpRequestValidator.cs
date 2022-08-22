using FluentValidation;
using Gateway.Root.PersonalData.Presentation.PersonalData.HttpModels;

namespace Gateway.Root.PersonalData.Presentation.PersonalData.Validator;

public class RequestApprovalValidator : AbstractValidator<RequestApprovalRequest>
{
    public RequestApprovalValidator()
    {
        RuleFor(request => request.FirstName).NotEmpty();
        RuleFor(request => request.LastName).NotEmpty();
        RuleFor(request => request.Country).NotEmpty();
        RuleFor(request => request.City).NotEmpty();
        RuleFor(request => request.Files).NotNull();
    }
}
