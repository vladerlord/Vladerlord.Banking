using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BankingGateway.Shared;

public class ValidateModelStateAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid) return;

        var errors = new Dictionary<string, List<string>>();

        foreach (var (key, modelState) in context.ModelState)
        {
            if (modelState.Errors.Count == 0)
                continue;

            if (!errors.ContainsKey(key))
                errors.Add(key, new List<string>());

            foreach (var modelStateError in modelState.Errors)
                errors[key].Add(modelStateError.ErrorMessage);
        }

        var response = new ValidationErrorResponse(errors);

        context.Result = new JsonResult(response)
        {
            StatusCode = StatusCodes.Status400BadRequest
        };
    }
}