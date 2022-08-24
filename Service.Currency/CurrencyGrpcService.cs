using Service.Currency.Abstraction;
using Shared.Grpc;
using Shared.Grpc.Currency;
using Shared.Grpc.Currency.Contract;

namespace Service.Currency;

public class CurrencyGrpcService : ICurrencyGrpcService
{
    private readonly ICurrencyRepository _currencyRepository;

    public CurrencyGrpcService(ICurrencyRepository currencyRepository)
    {
        _currencyRepository = currencyRepository;
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
}
