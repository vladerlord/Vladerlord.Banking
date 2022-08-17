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

	public UserController(IIdentityGrpcService identityGrpcService, UserResetService userResetService)
	{
		_identityGrpcService = identityGrpcService;
		_userResetService = userResetService;
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginUserHttpRequest request)
	{
		var response = await _identityGrpcService.LoginAsync(request.ToGrpcRequest());
		var httpResponse = new LoginUserHttpResponse(response.Status, response.Jwt);

		return new JsonResult(httpResponse)
		{
			StatusCode = httpResponse.StatusCode
		};
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] RegisterUserHttpRequest request)
	{
		var confirmationCode = _userResetService.GenerateConfirmationCode();
		var confirmationUrl = _userResetService.BuildUserRegistrationConfirmationBaseUri(confirmationCode);
		var grpcRequest = request.ToGrpcRequest(confirmationCode, confirmationUrl);

		var response = await _identityGrpcService.RegisterAsync(grpcRequest);
		var httpResponse = new RegisterUserHttpResponse(response.Status);

		return new JsonResult(httpResponse)
		{
			StatusCode = httpResponse.StatusCode
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