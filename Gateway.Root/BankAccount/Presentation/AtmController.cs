using Chassis.Gateway;
using Chassis.Gateway.ApiResponse;
using Gateway.Root.BankAccount.Presentation.HttpModel;
using Gateway.Root.Shared;
using Microsoft.AspNetCore.Mvc;
using Shared.Abstractions;
using Shared.Grpc;
using Shared.Grpc.BankAccount;
using Shared.Grpc.BankAccount.Contract;
using Shared.Grpc.Currency;
using Shared.Grpc.Currency.Contract;
using Shared.Grpc.PersonalData;
using Shared.Grpc.PersonalData.Contracts;

namespace Gateway.Root.BankAccount.Presentation;

[ApiController]
[JwtAuthentication]
[PermissionRequired(UserStatus.Atm)]
[Route("atm")]
public class AtmAccountController : ControllerBase
{
    private readonly IBankAccountGrpcService _bankAccountGrpcService;
    private readonly IPersonalDataGrpcService _personalDataGrpcService;
    private readonly ICurrencyGrpcService _currencyGrpcService;

    public AtmAccountController(IBankAccountGrpcService bankAccountGrpcService,
        IPersonalDataGrpcService personalDataGrpcService, ICurrencyGrpcService currencyGrpcService)
    {
        _bankAccountGrpcService = bankAccountGrpcService;
        _personalDataGrpcService = personalDataGrpcService;
        _currencyGrpcService = currencyGrpcService;
    }

    [HttpPost("deposit"), ApiResponseWrapper]
    public async Task<IActionResult> Deposit([FromBody] DepositRequest request)
    {
        var bankAccount = await _bankAccountGrpcService.GetByIdAsync(new GetBankAccountByIdGrpcRequest
        {
            BankAccountId = Guid.Parse(request.BankAccountId)
        });
        var currentUserPersonalData = await _personalDataGrpcService.GetByUserIdAsync(
            new GetByUserIdPersonalDataByIdGrpcRequest
            {
                UserId = HttpContext.GetUserId()
            });
        var currency = await _currencyGrpcService.GetByCodeAsync(new GetByCodeGrpcRequest
        {
            Code = request.Currency
        });

        if (bankAccount.BankAccount == null)
            return new JsonResult(new DepositResponse { Status = bankAccount.GrpcResponse.Status.ToString() })
            {
                StatusCode = bankAccount.GrpcResponse.Status.ToHttpCode()
            };

        if (currentUserPersonalData.PersonalData == null)
            return new JsonResult(new DepositResponse { Status = bankAccount.GrpcResponse.Status.ToString() })
            {
                StatusCode = bankAccount.GrpcResponse.Status.ToHttpCode()
            };

        if (bankAccount.BankAccount.PersonalDataId != currentUserPersonalData.PersonalData.Id)
            return new JsonResult(new DepositResponse { Status = GrpcResponseStatus.Invalid.ToString() })
            {
                StatusCode = GrpcResponseStatus.Invalid.ToHttpCode()
            };

        if (currency.Currency == null)
            return new JsonResult(new DepositResponse { Status = currency.GrpcResponse.Status.ToString() })
            {
                StatusCode = currency.GrpcResponse.Status.ToHttpCode()
            };

        var grpcResponse = await _bankAccountGrpcService.AddFundsAsync(request.ToGrpcRequest());

        return new JsonResult(new DepositResponse { Status = grpcResponse.GrpcResponse.Status.ToString() })
        {
            StatusCode = grpcResponse.GrpcResponse.Status.ToHttpCode()
        };
    }

    [HttpPost("withdraw"), ApiResponseWrapper]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest request)
    {
        var bankAccount = await _bankAccountGrpcService.GetByIdAsync(new GetBankAccountByIdGrpcRequest
        {
            BankAccountId = Guid.Parse(request.BankAccountId)
        });
        var currentUserPersonalData = await _personalDataGrpcService.GetByUserIdAsync(
            new GetByUserIdPersonalDataByIdGrpcRequest
            {
                UserId = HttpContext.GetUserId()
            });
        var currency = await _currencyGrpcService.GetByCodeAsync(new GetByCodeGrpcRequest
        {
            Code = request.Currency
        });

        if (bankAccount.BankAccount == null)
            return new JsonResult(new DepositResponse { Status = bankAccount.GrpcResponse.Status.ToString() })
            {
                StatusCode = bankAccount.GrpcResponse.Status.ToHttpCode()
            };

        if (currentUserPersonalData.PersonalData == null)
            return new JsonResult(new DepositResponse { Status = bankAccount.GrpcResponse.Status.ToString() })
            {
                StatusCode = bankAccount.GrpcResponse.Status.ToHttpCode()
            };

        if (bankAccount.BankAccount.PersonalDataId != currentUserPersonalData.PersonalData.Id)
            return new JsonResult(new DepositResponse { Status = GrpcResponseStatus.Invalid.ToString() })
            {
                StatusCode = GrpcResponseStatus.Invalid.ToHttpCode()
            };

        if (currency.Currency == null)
            return new JsonResult(new DepositResponse { Status = currency.GrpcResponse.Status.ToString() })
            {
                StatusCode = currency.GrpcResponse.Status.ToHttpCode()
            };

        var grpcResponse = await _bankAccountGrpcService.TakeFundsAsync(request.ToGrpcRequest());

        return new JsonResult(new DepositResponse { Status = grpcResponse.GrpcResponse.Status.ToString() })
        {
            StatusCode = grpcResponse.GrpcResponse.Status.ToHttpCode()
        };
    }
}
