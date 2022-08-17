using Chassis.Gateway;
using Gateway.Root.PersonalData.Application;
using Gateway.Root.PersonalData.Presentation.Models;
using Gateway.Root.Shared;
using Microsoft.AspNetCore.Mvc;
using Shared.Grpc;

namespace Gateway.Root.PersonalData.Presentation;

[ApiController]
[JwtAuthentication, AdminPermissionRequired]
[Route("personal-data/management")]
public class PersonalDataManagementController : ControllerBase
{
	private readonly PersonalDataManagementService _personalDataManagementService;

	public PersonalDataManagementController(PersonalDataManagementService personalDataManagementService)
	{
		_personalDataManagementService = personalDataManagementService;
	}

	[HttpGet("list-all-unapproved")]
	public async Task<IActionResult> ListAllUnapproved()
	{
		var response = await _personalDataManagementService.ListAllUnapproved();
		var httpResponse = new ListAllUnapprovedHttpResponse(response);

		return new JsonResult(httpResponse) { StatusCode = httpResponse.StatusCode };
	}

	[HttpGet("{personalDataId}")]
	public async Task<IActionResult> GetById([FromRoute] GetPersonalDataByIdHttpRequest request)
	{
		var appResponse = await _personalDataManagementService.GetPersonalDataById(request.PersonalDataId);
		var httpResponse = new GetPersonalDataByIdHttpResponse(appResponse);

		return new JsonResult(httpResponse)
		{
			StatusCode = httpResponse.StatusCode
		};
	}

	[HttpPost("approve/{personalDataId}")]
	public async Task<IActionResult> Approve([FromRoute] ApprovePersonalDataHttpRequest request)
	{
		var result = await _personalDataManagementService.ApproveAsync(request.PersonalDataId);

		var httpResponse = new ApprovePersonalDataHttpResponse(result.ToString(), result.ToHttpCode());

		return new JsonResult(httpResponse)
		{
			StatusCode = httpResponse.StatusCode
		};
	}

	[HttpPost("decline/{personalDataId}")]
	public async Task<IActionResult> Decline([FromRoute] DeclinePersonalDataHttpRequest request)
	{
		var result = await _personalDataManagementService.DeclineAsync(request.PersonalDataId);

		var httpResponse = new DeclinePersonalDataHttpResponse(result.ToString(), result.ToHttpCode());

		return new JsonResult(httpResponse)
		{
			StatusCode = httpResponse.StatusCode
		};
	}
}
