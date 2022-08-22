using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace Chassis.Gateway.ApiResponse;

public class HandleExceptionAttribute : ExceptionFilterAttribute
{
    private readonly HandleExceptionOptions _options;

    public HandleExceptionAttribute(IOptions<HandleExceptionOptions> options)
    {
        _options = options.Value;
    }

    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is ValidationErrorException exception)
            context.Result = HandleValidationErrorException(exception.ModelState);
        else
            context.Result = HandleException(context.Exception);

        context.ExceptionHandled = true;

        base.OnException(context);
    }

    private JsonResult HandleValidationErrorException(ModelStateDictionary modelStateDictionary)
    {
        var errors = new Dictionary<string, List<string>>();

        foreach (var (key, modelState) in modelStateDictionary)
        {
            if (modelState.Errors.Count == 0)
                continue;

            if (!errors.ContainsKey(key))
                errors.Add(key, new List<string>());

            foreach (var modelStateError in modelState.Errors)
                errors[key].Add(modelStateError.ErrorMessage);
        }

        var result = new ValidationApiResponse(errors, _options.ApiVersion);

        return new JsonResult(result) { StatusCode = result.StatusCode };
    }

    private JsonResult HandleException(Exception exception)
    {
        var result = new ExceptionApiResponse(_options.ApiVersion)
        {
            ExceptionType = exception.GetType().ToString(),
            ExceptionMessage = exception.Message
        };

        return new JsonResult(result) { StatusCode = result.StatusCode };
    }
}
