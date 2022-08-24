using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
            context.Result = HandleValidationErrorException(exception);
        else
            context.Result = HandleException(context.Exception);

        context.ExceptionHandled = true;

        base.OnException(context);
    }

    private JsonResult HandleValidationErrorException(ValidationErrorException exception)
    {
        var result = new ValidationApiResponse(exception.Errors, _options.ApiVersion);

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
