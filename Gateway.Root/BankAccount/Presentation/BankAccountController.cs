using Chassis.Gateway.ApiResponse;
using Gateway.Root.BankAccount.Domain;
using Gateway.Root.BankAccount.Presentation.HttpModel;
using Gateway.Root.Shared;
using Microsoft.AspNetCore.Mvc;
using Shared.Grpc;
using Shared.Grpc.BankAccount;
using Shared.Grpc.BankAccount.Contract;
using Shared.Grpc.Currency;
using Shared.Grpc.Currency.Contract;
using Shared.Grpc.PersonalData;
using Shared.Grpc.PersonalData.Contracts;

namespace Gateway.Root.BankAccount.Presentation;

[ApiController]
[Route("bank-account")]
public class BankAccountController : ControllerBase
{
    private readonly IBankAccountGrpcService _bankAccountGrpcService;
    private readonly IPersonalDataGrpcService _personalDataGrpcService;
    private readonly ICurrencyGrpcService _currencyGrpcService;

    public BankAccountController(IBankAccountGrpcService bankAccountGrpcService,
        IPersonalDataGrpcService personalDataGrpcService, ICurrencyGrpcService currencyGrpcService)
    {
        _bankAccountGrpcService = bankAccountGrpcService;
        _personalDataGrpcService = personalDataGrpcService;
        _currencyGrpcService = currencyGrpcService;
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
        var currency = await _currencyGrpcService.GetByCodeAsync(new GetByCodeGrpcRequest
        {
            Code = request.CurrencyCode
        });

        if (personalData.GrpcResponse.Status != GrpcResponseStatus.Ok || personalData.PersonalData == null)
        {
            var exception = new ValidationErrorException();
            exception.AddError("PersonalDataId", "Personal data doesn't exist for current user");

            throw exception;
        }

        if (currency.GrpcResponse.Status != GrpcResponseStatus.Ok || currency.Currency == null)
        {
            var exception = new ValidationErrorException();
            exception.AddError("CurrencyCode", "Currency doesn't exist");

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

    [JwtAuthentication]
    [HttpPost("transfer"), ApiResponseWrapper]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
    {
        var currentUserPersonalData = await _personalDataGrpcService.GetByUserIdAsync(
            new GetByUserIdPersonalDataByIdGrpcRequest
            {
                UserId = HttpContext.GetUserId()
            });
        var fromBankAccount = await _bankAccountGrpcService.GetByIdAsync(new GetBankAccountByIdGrpcRequest
        {
            BankAccountId = Guid.Parse(request.FromBankAccountId)
        });
        var toBankAccount = await _bankAccountGrpcService.GetByIdAsync(new GetBankAccountByIdGrpcRequest
        {
            BankAccountId = Guid.Parse(request.ToBankAccountId)
        });

        if (currentUserPersonalData.PersonalData == null)
        {
            var response = new DepositResponse { Status = currentUserPersonalData.GrpcResponse.Status.ToString() };

            return new JsonResult(response)
            {
                StatusCode = currentUserPersonalData.GrpcResponse.Status.ToHttpCode()
            };
        }

        if (fromBankAccount.BankAccount == null)
        {
            var response = new DepositResponse { Status = fromBankAccount.GrpcResponse.Status.ToString() };

            return new JsonResult(response)
            {
                StatusCode = fromBankAccount.GrpcResponse.Status.ToHttpCode()
            };
        }

        if (toBankAccount.BankAccount == null)
        {
            var response = new DepositResponse { Status = toBankAccount.GrpcResponse.Status.ToString() };

            return new JsonResult(response)
            {
                StatusCode = toBankAccount.GrpcResponse.Status.ToHttpCode()
            };
        }

        if (currentUserPersonalData.PersonalData.Id != fromBankAccount.BankAccount.PersonalDataId)
        {
            var response = new DepositResponse { Status = GrpcResponseStatus.Invalid.ToString() };

            return new JsonResult(response)
            {
                StatusCode = GrpcResponseStatus.Invalid.ToHttpCode()
            };
        }

        var grpcResponse = await _bankAccountGrpcService.TransferFundsAsync(request.ToGrpcRequest());

        return new JsonResult(new TransferResponse { Status = grpcResponse.GrpcResponse.Status.ToString() })
        {
            StatusCode = grpcResponse.GrpcResponse.Status.ToHttpCode()
        };
    }
}
