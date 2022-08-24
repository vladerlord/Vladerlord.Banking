using Chassis.Gateway.ApiResponse;
using Gateway.Root.BankAccount.Domain;
using Gateway.Root.BankAccount.Presentation.HttpModel;
using Gateway.Root.Shared;
using Microsoft.AspNetCore.Mvc;
using Shared.Grpc;
using Shared.Grpc.BankAccount;
using Shared.Grpc.PersonalData;
using Shared.Grpc.PersonalData.Contracts;

namespace Gateway.Root.BankAccount.Presentation;

[ApiController]
[Route("bank-account")]
public class BankAccountController : ControllerBase
{
    private readonly IBankAccountGrpcService _bankAccountGrpcService;
    private readonly IPersonalDataGrpcService _personalDataGrpcService;

    public BankAccountController(IBankAccountGrpcService bankAccountGrpcService,
        IPersonalDataGrpcService personalDataGrpcService)
    {
        _bankAccountGrpcService = bankAccountGrpcService;
        _personalDataGrpcService = personalDataGrpcService;
    }

    [JwtAuthentication]
    [HttpPost, ApiResponseWrapper]
    public async Task<IActionResult> Create([FromBody] CreateRequest request)
    {
        var currentUserId = HttpContext.GetUserId();
        
        var personalData = await _personalDataGrpcService.GetByUserIdAsync(new GetByUserIdPersonalDataByIdGrpcRequest
        {
            UserId = currentUserId
        });
        
        if (personalData.GrpcResponse.Status == GrpcResponseStatus.NotFound || personalData.PersonalData == null)
        {
            var exception = new ValidationErrorException();
            exception.AddError("PersonalDataId", "Personal data doesn't exist for current user");
        
            throw exception;
        }

        var grpcResponse =
            await _bankAccountGrpcService.CreateAsync(request.ToGrpcRequest(personalData.PersonalData.Id));

        var httpResponse = new CreateResponse
        {
            Status = grpcResponse.GrpcResponse.Status.ToString(),
            BankAccount = grpcResponse.BankAccount?.ToDto()
        };
        
        return new JsonResult(httpResponse)
        {
            StatusCode = grpcResponse.GrpcResponse.Status.ToHttpCode()
        };
    }
}
