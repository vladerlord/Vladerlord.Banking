using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace Chassis.Gateway.ApiResponse;

public class ApiResponseWrapperMiddleware
{
    private readonly RequestDelegate _next;

    public ApiResponseWrapperMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IOptions<HandleExceptionOptions> options, ILogger logger)
    {
        var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
        var attribute = endpoint?.Metadata.GetMetadata<ApiResponseWrapperAttribute>();

        if (attribute == null)
        {
            await HandleSystemException(context, options, logger);

            return;
        }

        var currentBody = context.Response.Body;

        using var memoryStream = new MemoryStream();

        context.Response.Body = memoryStream;

        await HandleSystemException(context, options, logger, currentBody);

        if (context.Response.HasStarted) return;

        context.Response.Body = currentBody;
        memoryStream.Seek(0, SeekOrigin.Begin);

        var handledByExceptionHandler = context.Response.StatusCode is 422 or 500;
        var contentAsString = await new StreamReader(memoryStream).ReadToEndAsync();

        if (handledByExceptionHandler)
        {
            await context.Response.WriteAsync(contentAsString);
            return;
        }

        var contentAsObject = JsonConvert.DeserializeObject(contentAsString);

        if (contentAsObject != null)
        {
            var response = new SuccessApiResponse<object>(contentAsObject, options.Value.ApiVersion,
                context.Response.StatusCode);
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
        else
        {
            await context.Response.WriteAsync(contentAsString);
        }
    }

    private async Task HandleSystemException(HttpContext context, IOptions<HandleExceptionOptions> options,
        ILogger logger, Stream? stream = null)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            logger.Error("{Message}", e.ToString());

            var response = new ExceptionApiResponse(options.Value.ApiVersion, context.Response.StatusCode)
            {
                ExceptionType = e.GetType().ToString(),
                ExceptionMessage = e.Message
            };

            if (stream != null) context.Response.Body = stream;

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
