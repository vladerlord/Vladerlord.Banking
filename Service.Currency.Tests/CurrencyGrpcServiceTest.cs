using Microsoft.Extensions.Logging;
using Moq;
using Service.Currency.Abstraction;
using Service.Currency.Exceptions;
using Service.Currency.Model;
using Shared.Grpc;
using Shared.Grpc.Currency.Contract;

namespace Service.Currency.Tests;

public class CurrencyGrpcServiceTest
{
    private Mock<ICurrencyRepository> _currencyRepository = null!;
    private Mock<ICurrencyExchanger> _currencyExchanger = null!;
    private Mock<ILogger<CurrencyGrpcService>> _logger = null!;
    private CurrencyGrpcService _currencyGrpcService = null!;

    [SetUp]
    public void Setup()
    {
        _currencyRepository = new Mock<ICurrencyRepository>();
        _currencyExchanger = new Mock<ICurrencyExchanger>();
        _logger = new Mock<ILogger<CurrencyGrpcService>>();

        _currencyGrpcService =
            new CurrencyGrpcService(_currencyRepository.Object, _currencyExchanger.Object, _logger.Object);
    }

    [Test]
    public async Task GetAllSuccessful()
    {
        // Arrange
        const string code1 = "USD";
        const decimal exchangeRate1 = 1.231421m;

        var currenciesModels = new List<CurrencyDatabaseModel>
        {
            new(code1, exchangeRate1)
        };

        _currencyRepository.Setup(r => r.GetAllAsync())
            .Returns(Task.FromResult<IEnumerable<CurrencyDatabaseModel>>(currenciesModels));

        // Act
        var response = await _currencyGrpcService.GetAllAsync();

        // Assert
        var currenciesList = response.Currencies.ToList();

        Assert.Multiple(() =>
        {
            Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.Ok));
            Assert.That(currenciesList[0].Code, Is.EqualTo(code1));
            Assert.That(currenciesList[0].ExchangeRateToUsd, Is.EqualTo(exchangeRate1));
        });
    }

    [Test]
    public async Task GetAllError()
    {
        // Arrange
        _currencyRepository.Setup(r => r.GetAllAsync())
            .Throws<Exception>();

        // Act
        var response = await _currencyGrpcService.GetAllAsync();

        // Assert
        var currenciesList = response.Currencies.ToList();

        Assert.Multiple(() =>
        {
            Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.Error));
            Assert.That(currenciesList, Is.Empty);
        });
    }

    [Test]
    public async Task GetByCodeSuccessful()
    {
        // Arrange
        const string code = "USD";
        const decimal exchangeRate = 1.231421m;
        var currencyDatabaseModel = new CurrencyDatabaseModel(code, exchangeRate);
        var request = new GetByCodeGrpcRequest { Code = code };

        _currencyRepository.Setup(r => r.FindByCodeAsync(code))
            .Returns(Task.FromResult<CurrencyDatabaseModel?>(currencyDatabaseModel));

        // Act
        var response = await _currencyGrpcService.GetByCodeAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.Ok));
            Assert.That(response.Currency?.Code, Is.EqualTo(code));
            Assert.That(response.Currency?.ExchangeRateToUsd, Is.EqualTo(exchangeRate));
        });
    }

    [Test]
    public async Task GetByCodeNotFound()
    {
        // Arrange
        const string code = "USD";
        var request = new GetByCodeGrpcRequest { Code = code };

        _currencyRepository.Setup(r => r.FindByCodeAsync(code))
            .Returns(Task.FromResult<CurrencyDatabaseModel?>(null));

        // Act
        var response = await _currencyGrpcService.GetByCodeAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.NotFound));
            Assert.That(response.Currency, Is.Null);
        });
    }

    [Test]
    public async Task GetByCodeError()
    {
        // Arrange
        const string code = "USD";
        var request = new GetByCodeGrpcRequest { Code = code };

        _currencyRepository.Setup(r => r.FindByCodeAsync(code))
            .Throws<Exception>();

        // Act
        var response = await _currencyGrpcService.GetByCodeAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.Error));
            Assert.That(response.Currency, Is.Null);
        });
    }

    [Test]
    public async Task ExchangeSuccessful()
    {
        // Arrange
        const string fromCurrencyCode = "USD";
        const string toCurrencyCode = "UAH";
        const decimal amount = 100.25m;
        var request = new ExchangeGrpcRequest
        {
            FromCurrencyCode = fromCurrencyCode,
            ToCurrencyCode = toCurrencyCode,
            Amount = amount
        };

        _currencyExchanger.Setup(c => c.Exchange(fromCurrencyCode, toCurrencyCode, amount))
            .Returns(Task.FromResult(2700m));

        // Act
        var response = await _currencyGrpcService.ExchangeAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.Ok));
            Assert.That(response.Amount, Is.EqualTo(2700m));
        });
    }

    [Test]
    public async Task ExchangeCurrencyNotFound()
    {
        // Arrange
        const string fromCurrencyCode = "USD";
        const string toCurrencyCode = "UAH";
        const decimal amount = 100.25m;
        var request = new ExchangeGrpcRequest
        {
            FromCurrencyCode = fromCurrencyCode,
            ToCurrencyCode = toCurrencyCode,
            Amount = amount
        };

        _currencyExchanger.Setup(c => c.Exchange(fromCurrencyCode, toCurrencyCode, amount))
            .Throws<EntityNotFoundException>();

        // Act
        var response = await _currencyGrpcService.ExchangeAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.NotFound));
            Assert.That(response.Amount, Is.Null);
        });
    }

    [Test]
    public async Task ExchangeExchangeException()
    {
        // Arrange
        const string fromCurrencyCode = "USD";
        const string toCurrencyCode = "UAH";
        const decimal amount = 100.25m;
        var request = new ExchangeGrpcRequest
        {
            FromCurrencyCode = fromCurrencyCode,
            ToCurrencyCode = toCurrencyCode,
            Amount = amount
        };

        _currencyExchanger.Setup(c => c.Exchange(fromCurrencyCode, toCurrencyCode, amount))
            .Throws<Exception>();

        // Act
        var response = await _currencyGrpcService.ExchangeAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.Error));
            Assert.That(response.Amount, Is.Null);
        });
    }
}
