using Chassis.Gateway.ApiResponse;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Gateway.Root.Shared;

public class ValidateModelStateAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid) return;

        throw new ValidationErrorException(context.ModelState);
    }
}
