using Gateway.Root.PersonalData.Application;
using Gateway.Root.PersonalData.Presentation.PersonalData.HttpModels;
using Gateway.Root.Shared;
using Microsoft.AspNetCore.Mvc;
using Shared.Grpc;

namespace Gateway.Root.PersonalData.Presentation.PersonalData;

[ApiController]
[Route("personal-data")]
public class PersonalDataController : ControllerBase
{
    private readonly PersonalDataService _personalDataService;

    public PersonalDataController(PersonalDataService personalDataService)
    {
        _personalDataService = personalDataService;
    }

    [JwtAuthentication]
    [HttpPost("request-approval")]
    public async Task<IActionResult> RequestApproval([FromForm] RequestApprovalRequest requestApprovalRequest)
    {
        var userId = HttpContext.GetUserId();
        var appDto = requestApprovalRequest.ToPersonalDataConfirmationDto(userId);

        var appResponse = await _personalDataService.SendPersonalDataConfirmationRequest(appDto);

        var httpResponse = new RequestApprovalResponse
        {
            Status = appResponse.GrpcStatus.Status.ToString(),
            PersonalData = appResponse.Content
        };

        return new JsonResult(httpResponse)
        {
            StatusCode = appResponse.GrpcStatus.Status.ToHttpCode()
        };
    }
}
