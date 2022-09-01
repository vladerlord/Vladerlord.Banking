using Service.BankAccount.Abstraction;
using Service.BankAccount.Model;
using Shared.Grpc;
using Shared.Grpc.BankAccount;
using Shared.Grpc.BankAccount.Contract;
using Shared.Grpc.Currency;
using Shared.Grpc.Currency.Contract;

namespace Service.BankAccount;

public class BankAccountGrpcService : IBankAccountGrpcService
{
    private readonly IBankAccountRepository _bankAccountRepository;
    private readonly ICurrencyGrpcService _currencyGrpcService;
    private readonly ILogger<BankAccountGrpcService> _logger;

    public BankAccountGrpcService(IBankAccountRepository bankAccountRepository,
        ICurrencyGrpcService currencyGrpcService, ILogger<BankAccountGrpcService> logger)
    {
        _bankAccountRepository = bankAccountRepository;
        _currencyGrpcService = currencyGrpcService;
        _logger = logger;
    }

    public async Task<CreateBankAccountGrpcResponse> CreateAsync(CreateBankAccountGrpcRequest request)
    {
        var status = GrpcResponseStatus.Ok;
        BankAccountDatabaseModel? bankAccount = null;

        try
        {
            bankAccount = await _bankAccountRepository.CreateAsync(request.ToDatabaseModel(0m));
        }
        catch (Exception e)
        {
            _logger.LogCritical("{Error}", e.ToString());
            status = GrpcResponseStatus.Error;
        }

        return new CreateBankAccountGrpcResponse
        {
            GrpcResponse = new GrpcResponse { Status = status },
            BankAccount = bankAccount?.ToGrpcModel()
        };
    }

    public async Task<GetBankAccountByIdGrpcResponse> GetByIdAsync(GetBankAccountByIdGrpcRequest request)
    {
        var bankAccount = await _bankAccountRepository.FindByIdAsync(request.BankAccountId);

        return new GetBankAccountByIdGrpcResponse
        {
            GrpcResponse = new GrpcResponse
            {
                Status = bankAccount != null ? GrpcResponseStatus.Ok : GrpcResponseStatus.NotFound,
            },
            BankAccount = bankAccount?.ToGrpcModel()
        };
    }

    public async Task<AddFundsGrpcResponse> AddFundsAsync(AddFundsGrpcRequest request)
    {
        var bankAccount = await _bankAccountRepository.FindByIdAsync(request.BankAccountId);
        var amount = request.Amount;
        var status = GrpcResponseStatus.Ok;

        if (bankAccount == null)
        {
            return new AddFundsGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.BankAccountNotFound }
            };
        }

        if (bankAccount.CurrencyCode != request.Currency)
        {
            var currencyResponse = await _currencyGrpcService.ExchangeAsync(new ExchangeGrpcRequest
            {
                FromCurrencyCode = request.Currency,
                ToCurrencyCode = bankAccount.CurrencyCode,
                Amount = request.Amount
            });

            if (currencyResponse.Amount == null || currencyResponse.GrpcResponse.Status != GrpcResponseStatus.Ok)
            {
                return new AddFundsGrpcResponse
                {
                    GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.CurrencyExchangeError }
                };
            }

            amount = currencyResponse.Amount.Value;
        }

        _logger.LogInformation("Trying to add funds to: {BankAccountId}, amount: {Amount}, currency: {RequestCurrency}",
            bankAccount.Id, amount, request.Currency);

        try
        {
            await _bankAccountRepository.AddToBalanceAsync(bankAccount.Id, amount);
        }
        catch (Exception e)
        {
            _logger.LogCritical("Add funds error: {Error}", e.ToString());
            status = GrpcResponseStatus.Error;
        }

        return new AddFundsGrpcResponse
        {
            GrpcResponse = new GrpcResponse { Status = status }
        };
    }

    public async Task<TakeFundsGrpcResponse> TakeFundsAsync(TakeFundsGrpcRequest request)
    {
        var bankAccount = await _bankAccountRepository.FindByIdAsync(request.BankAccountId);

        if (bankAccount == null)
        {
            return new TakeFundsGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.NotFound }
            };
        }

        var amount = request.Amount;

        if (bankAccount.CurrencyCode != request.Currency)
        {
            var currencyResponse = await _currencyGrpcService.ExchangeAsync(new ExchangeGrpcRequest
            {
                FromCurrencyCode = request.Currency,
                ToCurrencyCode = bankAccount.CurrencyCode,
                Amount = request.Amount
            });

            if (currencyResponse.Amount == null || currencyResponse.GrpcResponse.Status != GrpcResponseStatus.Ok)
            {
                return new TakeFundsGrpcResponse
                {
                    GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.CurrencyExchangeError }
                };
            }

            amount = currencyResponse.Amount.Value;
        }

        _logger.LogInformation(
            "Trying to take funds from: {BankAccountId}, amount: {Amount}, currency: {RequestCurrency}",
            bankAccount.Id, amount, request.Currency);

        try
        {
            await _bankAccountRepository.TakeFromBalanceAsync(bankAccount.Id, amount);
        }
        catch (Exception e)
        {
            _logger.LogCritical("Error when trying to take funds: {Error}", e.ToString());

            return new TakeFundsGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Error }
            };
        }

        return new TakeFundsGrpcResponse
        {
            GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Ok }
        };
    }

    public async Task<TransferFundsGrpcResponse> TransferFundsAsync(TransferFundsGrpcRequest request)
    {
        var fromBankAccount = await _bankAccountRepository.FindByIdAsync(request.FromBankAccountId);
        var toBankAccount = await _bankAccountRepository.FindByIdAsync(request.ToBankAccountId);

        if (fromBankAccount == null || toBankAccount == null)
        {
            return new TransferFundsGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.NotFound }
            };
        }

        var fromAmount = request.Amount;
        var toAmount = request.Amount;

        if (fromBankAccount.CurrencyCode != toBankAccount.CurrencyCode)
        {
            var currencyResponse = await _currencyGrpcService.ExchangeAsync(new ExchangeGrpcRequest
            {
                FromCurrencyCode = fromBankAccount.CurrencyCode,
                ToCurrencyCode = toBankAccount.CurrencyCode,
                Amount = request.Amount
            });

            if (currencyResponse.Amount == null || currencyResponse.GrpcResponse.Status != GrpcResponseStatus.Ok)
            {
                return new TransferFundsGrpcResponse
                {
                    GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.CurrencyExchangeError }
                };
            }

            toAmount = currencyResponse.Amount.Value;
        }

        _logger.LogInformation(
            "Trying to transfer funds from: {Account1} [{FromAmount} {Currency1}], to: {Account2}[{ToAmount} {Currency2}]",
            fromBankAccount.Id, fromAmount, fromBankAccount.CurrencyCode, toBankAccount.Id, toAmount,
            toBankAccount.CurrencyCode);

        try
        {
            await _bankAccountRepository.TransferAsync(fromBankAccount.Id, toBankAccount.Id, fromAmount, toAmount);
        }
        catch (Exception e)
        {
            _logger.LogCritical("Error when trying to take funds: {Error}", e.ToString());

            return new TransferFundsGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Error }
            };
        }

        return new TransferFundsGrpcResponse
        {
            GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Ok }
        };
    }
}
