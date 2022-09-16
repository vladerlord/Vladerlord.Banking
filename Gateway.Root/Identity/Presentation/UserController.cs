using System.Diagnostics.Metrics;
using Chassis;
using Chassis.Gateway.ApiResponse;
using Gateway.Root.Identity.Application;
using Gateway.Root.Identity.Presentation.Models;
using Gateway.Root.Identity.Presentation.Views;
using Microsoft.AspNetCore.Mvc;
using Shared.Grpc;
using Shared.Grpc.Identity;
using Shared.Grpc.Identity.Contracts;

namespace Gateway.Root.Identity.Presentation;

[ApiController]
[Route("[controller]")]
public class UserController : Controller
{
    private readonly IIdentityGrpcService _identityGrpcService;
    private readonly UserResetService _userResetService;
    private readonly UserService _userService;

    private static readonly Counter<int> RegistrationCounter =
        Metrics.Meter.CreateCounter<int>("banking_gateway_root_registration_counter");

    public UserController(IIdentityGrpcService identityGrpcService, UserResetService userResetService,
        UserService userService)
    {
        _identityGrpcService = identityGrpcService;
        _userResetService = userResetService;
        _userService = userService;
    }

    [HttpPost("login")]
    [ApiResponseWrapper]
    public async Task<IActionResult> Login([FromBody] LoginUserHttpRequest request)
    {
        var appResponse = await _userService.LoginAsync(request.Email, request.Password);

        var httpResponse = new LoginUserHttpResponse
        {
            Status = appResponse.GrpcStatus.Status.ToString(),
            Jwt = appResponse.Content.Jwt,
            User = appResponse.Content.User
        };

        return new JsonResult(httpResponse)
        {
            StatusCode = appResponse.GrpcStatus.Status.ToHttpCode()
        };
    }

    [HttpPost("register")]
    [ApiResponseWrapper]
    public async Task<IActionResult> Register([FromBody] RegisterUserHttpRequest request)
    {
        var confirmationCode = _userResetService.GenerateConfirmationCode();
        var confirmationUrl = _userResetService.BuildUserRegistrationConfirmationBaseUri(confirmationCode);

        var appResponse =
            await _userService.RegisterAsync(confirmationCode, confirmationUrl, request.Email, request.Password);

        var httpResponse = new RegisterUserHttpResponse
        {
            Status = appResponse.GrpcStatus.Status.ToString(),
            User = appResponse.Content
        };

        if (appResponse.GrpcStatus.Status == GrpcResponseStatus.Ok)
            RegistrationCounter.Add(1, new []{new KeyValuePair<string, object?>("email", request.Email)});

        return new JsonResult(httpResponse)
        {
            StatusCode = appResponse.GrpcStatus.Status.ToHttpCode()
        };
    }

    [ActionName("RegisterConfirmation")]
    [HttpGet("register-confirmation/{confirmationCode}")]
    public async Task<IActionResult> RegistrationConfirmation(string confirmationCode)
    {
        var response = await _identityGrpcService.RegisterConfirmationAsync(new RegisterConfirmationGrpcRequest
        {
            ConfirmationCode = confirmationCode
        });

        var viewModel = new UserRegistrationConfirmation
        {
            IsSuccess = response.Status == GrpcResponseStatus.Ok
        };

        return View("~/Identity/Presentation/Views/UserRegistrationConfirmation.cshtml", viewModel);
    }
}
