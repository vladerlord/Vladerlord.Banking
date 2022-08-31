using Chassis.Gateway;
using Gateway.Root.PersonalData.Presentation.KycScan.HttpModels;
using Gateway.Root.Shared;
using Microsoft.AspNetCore.Mvc;
using Shared.Abstractions;
using Shared.Grpc.PersonalData;
using Shared.Grpc.PersonalData.Contracts;

namespace Gateway.Root.PersonalData.Presentation.KycScan;

[ApiController]
[Route("[controller]")]
public class KycScanController : Controller
{
    private readonly IKycScanGrpcService _kycScanGrpcService;

    public KycScanController(IKycScanGrpcService kycScanGrpcService)
    {
        _kycScanGrpcService = kycScanGrpcService;
    }

    [JwtAuthentication]
    [PermissionRequired(UserStatus.Admin)]
    [HttpGet("{kycScanId}")]
    public async Task<IActionResult> GetById([FromRoute] GetByIdRequest request)
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
