using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Chassis.Gateway.ApiResponse;

public class ValidationErrorException : Exception
{
    public ModelStateDictionary ModelState { get; }

    public ValidationErrorException(ModelStateDictionary modelState)
    {
        ModelState = modelState;
    }
}
