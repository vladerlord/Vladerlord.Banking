using Chassis.Gateway;
using Gateway.Root.PersonalData.Presentation.Models;
using Gateway.Root.Shared;
using Microsoft.AspNetCore.Mvc;
using Shared.Grpc.PersonalData;
using Shared.Grpc.PersonalData.Contracts;

namespace Gateway.Root.PersonalData.Presentation;

[ApiController]
[Route("[controller]")]
public class KycScanController : Controller
{
	private readonly IKycScanGrpcService _kycScanGrpcService;

	public KycScanController(IKycScanGrpcService kycScanGrpcService)
	{
		_kycScanGrpcService = kycScanGrpcService;
	}

	[JwtAuthentication, AdminPermissionRequired]
	[HttpGet("{kycScanId}")]
	public async Task<IActionResult> Get([FromRoute] GetKycScanByIdHttpRequest request)
	{
		var grpcResponse = await _kycScanGrpcService.FindByIdAsync(new FindKycScanByIdGrpcRequest
		{
			Id = request.KycScanId
		});

		if (grpcResponse.KycScan == null)
			return NotFound();

		return new FileContentResult(grpcResponse.KycScan.Content, grpcResponse.KycScan.ContentType);
	}
}