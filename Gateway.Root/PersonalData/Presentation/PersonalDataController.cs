using Gateway.Root.PersonalData.Application;
using Gateway.Root.PersonalData.Presentation.Models;
using Gateway.Root.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Root.PersonalData.Presentation;

[ApiController]
[Route("[controller]")]
public class PersonalDataController : ControllerBase
{
	private readonly PersonalDataService _personalDataService;

	public PersonalDataController(PersonalDataService personalDataService)
	{
		_personalDataService = personalDataService;
	}

	[JwtAuthentication]
	[HttpPost("send-personal-data-confirmation")]
	public async Task<IActionResult> SendPersonalDataConfirmationRequest(
		[FromForm] SendPersonalDataConfirmationRequest request)
	{
		var userId = HttpContext.GetUserId();
		var requestDto = request.ToPersonalDataConfirmationDto(userId);

		var response = await _personalDataService.SendPersonalDataConfirmationRequest(requestDto);
		var httpResponse = new SendPersonalDataConfirmationResponse(response);

		return new JsonResult(httpResponse)
		{
			StatusCode = httpResponse.StatusCode
		};
	}
}