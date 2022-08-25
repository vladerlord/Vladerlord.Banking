using Service.Currency.Abstraction;
using Service.Currency.Service;
using Shared.Grpc;
using Shared.Grpc.Currency;
using Shared.Grpc.Currency.Contract;

namespace Service.Currency;

public class CurrencyGrpcService : ICurrencyGrpcService
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly CurrencyService _currencyService;
    private readonly ILogger<CurrencyGrpcService> _logger;

    public CurrencyGrpcService(ICurrencyRepository currencyRepository, CurrencyService currencyService,
        ILogger<CurrencyGrpcService> logger)
    {
        _currencyRepository = currencyRepository;
        _currencyService = currencyService;
        _logger = logger;
    }

    public async Task<GetAllGrpcResponse> GetAllAsync()
    {
        var databaseModels = await _currencyRepository.GetAllAsync();

        return new GetAllGrpcResponse
        {
            GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Ok },
            Currencies = databaseModels.Select(model => model.ToGrpcModel())
        };
    }

    public async Task<GetByCodeGrpcResponse> GetByCodeAsync(GetByCodeGrpcRequest request)
    {
        var currency = await _currencyRepository.FindByCodeAsync(request.Code);

        return new GetByCodeGrpcResponse
        {
            GrpcResponse = new GrpcResponse
            {
                Status = currency != null ? GrpcResponseStatus.Ok : GrpcResponseStatus.NotFound
            },
            Currency = currency?.ToGrpcModel()
        };
    }

    public async Task<ExchangeGrpcResponse> ExchangeAsync(ExchangeGrpcRequest request)
    {
        var fromCurrency = await _currencyRepository.FindByCodeAsync(request.FromCurrencyCode);
        var toCurrency = await _currencyRepository.FindByCodeAsync(request.ToCurrencyCode);

        if (fromCurrency == null || toCurrency == null)
            return new ExchangeGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.NotFound }
            };

        try
        {
            var result = _currencyService.Exchange(fromCurrency, toCurrency, request.Amount);

            return new ExchangeGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Ok },
                Amount = result
            };
        }
        catch (Exception e)
        {
            _logger.LogCritical("{Error}. ExchangeAsync, from: {From}, to: {To}, amount: {Amount}",
                e.ToString(), request.FromCurrencyCode, request.ToCurrencyCode, request.Amount);

            return new ExchangeGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Error }
            };
        }
    }
}
