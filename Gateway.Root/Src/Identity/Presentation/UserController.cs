using Gateway.Root.Identity.Presentation.Models;
using Gateway.Root.Shared;
using Microsoft.AspNetCore.Mvc;
using Shared.Abstractions.Grpc.Identity;

namespace Gateway.Root.Identity.Presentation;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IIdentityGrpcService _identityGrpcService;

    public UserController(IIdentityGrpcService identityGrpcService)
    {
        _identityGrpcService = identityGrpcService;
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
        var response = await _identityGrpcService.RegisterAsync(request.ToGrpcRequest());
        var httpResponse = new RegisterUserHttpResponse(response.Status);

        return new JsonResult(httpResponse)
        {
            StatusCode = httpResponse.StatusCode
        };
    }

    [JwtAuthentication]
    [HttpPost("check")]
    public async Task<IActionResult> Check()
    {
        return Ok();
    }
}