using Service.Currency.Abstraction;
using Service.Currency.Exceptions;
using Service.Currency.Model;
using Shared.Grpc;
using Shared.Grpc.Currency;
using Shared.Grpc.Currency.Contract;

namespace Service.Currency;

public class CurrencyGrpcService : ICurrencyGrpcService
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ICurrencyExchanger _currencyExchanger;
    private readonly ILogger<CurrencyGrpcService> _logger;

    public CurrencyGrpcService(ICurrencyRepository currencyRepository, ICurrencyExchanger currencyExchanger,
        ILogger<CurrencyGrpcService> logger)
    {
        _currencyRepository = currencyRepository;
        _currencyExchanger = currencyExchanger;
        _logger = logger;
    }

    public async Task<GetAllGrpcResponse> GetAllAsync()
    {
        var status = GrpcResponseStatus.Ok;
        IEnumerable<CurrencyDatabaseModel> databaseModels = Array.Empty<CurrencyDatabaseModel>();

        try
        {
            databaseModels = await _currencyRepository.GetAllAsync();
        }
        catch (Exception e)
        {
            _logger.LogCritical("{Error}", e.ToString());
            status = GrpcResponseStatus.Error;
        }

        return new GetAllGrpcResponse
        {
            GrpcResponse = new GrpcResponse { Status = status },
            Currencies = databaseModels.Select(model => model.ToGrpcModel())
        };
    }

    public async Task<GetByCodeGrpcResponse> GetByCodeAsync(GetByCodeGrpcRequest request)
    {
        try
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
        catch (Exception e)
        {
            _logger.LogCritical("Error while trying to find currency by: {Code}. {Error}", request.Code, e.ToString());

            return new GetByCodeGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Error },
            };
        }
    }

    public async Task<ExchangeGrpcResponse> ExchangeAsync(ExchangeGrpcRequest request)
    {
        try
        {
            var result =
                await _currencyExchanger.Exchange(request.FromCurrencyCode, request.ToCurrencyCode, request.Amount);

            return new ExchangeGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Ok },
                Amount = result
            };
        }
        catch (EntityNotFoundException)
        {
            return new ExchangeGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.NotFound }
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
