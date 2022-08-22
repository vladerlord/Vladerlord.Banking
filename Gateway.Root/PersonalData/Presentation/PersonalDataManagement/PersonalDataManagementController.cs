using Chassis.Gateway;
using Chassis.Gateway.ApiResponse;
using Gateway.Root.PersonalData.Application;
using Gateway.Root.PersonalData.Presentation.PersonalDataManagement.HttpModels;
using Gateway.Root.Shared;
using Microsoft.AspNetCore.Mvc;
using Shared.Grpc;

namespace Gateway.Root.PersonalData.Presentation.PersonalDataManagement;

[ApiController]
[JwtAuthentication]
[AdminPermissionRequired]
[Route("personal-data/management")]
public class PersonalDataManagementController : ControllerBase
{
    private readonly PersonalDataManagementService _personalDataManagementService;
    private readonly KycScanLinkBuilder _kycScanLinkBuilder;

    public PersonalDataManagementController(PersonalDataManagementService personalDataManagementService,
        KycScanLinkBuilder kycScanLinkBuilder)
    {
        _personalDataManagementService = personalDataManagementService;
        _kycScanLinkBuilder = kycScanLinkBuilder;
    }

    [HttpGet("{personalDataId}")]
    [ApiResponseWrapper]
    public async Task<IActionResult> GetById([FromRoute] GetByIdRequest request)
    {
        var appResponse = await _personalDataManagementService.GetPersonalDataById(request.PersonalDataId);

        var kycScansLinks = (from kycScanId in appResponse.Content?.KycScansIds
            select _kycScanLinkBuilder.BuildLinkToKycScan(kycScanId)).ToList();

        var httpResponse = new GetByIdResponse
        {
            Status = appResponse.GrpcStatus.Status.ToString(),
            PersonalData = appResponse.Content,
            KycScansLinks = kycScansLinks
        };

        return new JsonResult(httpResponse)
        {
            StatusCode = appResponse.GrpcStatus.Status.ToHttpCode()
        };
    }

    [HttpGet("unapproved")]
    [ApiResponseWrapper]
    public async Task<IActionResult> GetAllUnapproved()
    {
        var appResponse = await _personalDataManagementService.ListAllUnapproved();

        var httpResponse = new GetAllUnapprovedResponse
        {
            Status = appResponse.GrpcStatus.Status.ToString(),
            PersonalDataList = appResponse.Content
        };

        return new JsonResult(httpResponse) { StatusCode = appResponse.GrpcStatus.Status.ToHttpCode() };
    }

    [HttpPost("unapproved/approve/{personalDataId}")]
    [ApiResponseWrapper]
    public async Task<IActionResult> Approve([FromRoute] ApproveRequest request)
    {
        var appResult = await _personalDataManagementService.ApproveAsync(request.PersonalDataId);

        var httpResponse = new ApproveResponse
        {
            Status = appResult.GrpcStatus.Status.ToString()
        };

        return new JsonResult(httpResponse)
        {
            StatusCode = appResult.GrpcStatus.Status.ToHttpCode()
        };
    }

    [HttpPost("unapproved/decline/{personalDataId}")]
    [ApiResponseWrapper]
    public async Task<IActionResult> Decline([FromRoute] DeclineRequest request)
    {
        var appResponse = await _personalDataManagementService.DeclineAsync(request.PersonalDataId);

        var httpResponse = new DeclineResponse
        {
            Status = appResponse.GrpcStatus.Status.ToString()
        };

        return new JsonResult(httpResponse)
        {
            StatusCode = appResponse.GrpcStatus.Status.ToHttpCode()
        };
    }
}
