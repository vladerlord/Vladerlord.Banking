using Moq;
using Service.Currency.Abstraction;
using Service.Currency.Exceptions;
using Service.Currency.Model;
using Service.Currency.Service;

namespace Service.Currency.Tests;

public class DatabaseCurrencyExchangerTest
{
    private Mock<ICurrencyRepository> _currencyRepository = null!;
    private DatabaseCurrencyExchanger _currencyExchanger = null!;

    [SetUp]
    public void Setup()
    {
        _currencyRepository = new Mock<ICurrencyRepository>();
        _currencyExchanger = new DatabaseCurrencyExchanger(_currencyRepository.Object);
    }

    [Test]
    public async Task ToGreaterExchangeRateUahUsdSuccessful()
    {
        // Arrange
        const string fromCurrency = "UAH";
        const string toCurrency = "USD";
        var from = new CurrencyDatabaseModel(fromCurrency, 0.0271m);
        var to = new CurrencyDatabaseModel(toCurrency, 1m);
        const decimal amount = 1456.69m;

        _currencyRepository.Setup(r => r.FindByCodeAsync(fromCurrency))
            .Returns(Task.FromResult<CurrencyDatabaseModel?>(from));
        _currencyRepository.Setup(r => r.FindByCodeAsync(toCurrency))
            .Returns(Task.FromResult<CurrencyDatabaseModel?>(to));

        // Act
        var result = await _currencyExchanger.Exchange(fromCurrency, toCurrency, amount);

        // Assert
        Assert.That(result, Is.EqualTo(39.476299m));
    }

    [Test]
    public void ToGreaterExchangeRateUahUsdFromCurrencyNotFound()
    {
        // Arrange
        const string fromCurrency = "UAH";
        const string toCurrency = "USD";
        var to = new CurrencyDatabaseModel(toCurrency, 1m);
        const decimal amount = 1456.69m;

        _currencyRepository.Setup(r => r.FindByCodeAsync(fromCurrency))
            .Returns(Task.FromResult<CurrencyDatabaseModel?>(null));
        _currencyRepository.Setup(r => r.FindByCodeAsync(toCurrency))
            .Returns(Task.FromResult<CurrencyDatabaseModel?>(to));

        // Act & Assert
        Assert.ThrowsAsync<EntityNotFoundException>(async () =>
        {
            await _currencyExchanger.Exchange(fromCurrency, toCurrency, amount);
        });
    }

    [Test]
    public void ToGreaterExchangeRateUahUsdToCurrencyNotFound()
    {
        // Arrange
        const string fromCurrency = "UAH";
        const string toCurrency = "USD";
        var from = new CurrencyDatabaseModel(fromCurrency, 0.0271m);
        const decimal amount = 1456.69m;

        _currencyRepository.Setup(r => r.FindByCodeAsync(fromCurrency))
            .Returns(Task.FromResult<CurrencyDatabaseModel?>(from));
        _currencyRepository.Setup(r => r.FindByCodeAsync(toCurrency))
            .Returns(Task.FromResult<CurrencyDatabaseModel?>(null));

        // Act & Assert
        Assert.ThrowsAsync<EntityNotFoundException>(async () =>
        {
            await _currencyExchanger.Exchange(fromCurrency, toCurrency, amount);
        });
    }

    [Test]
    public async Task ToGreaterExchangeRateUahGbSuccessful()
    {
        // Arrange
        const string fromCurrency = "UAH";
        const string toCurrency = "USD";
        var from = new CurrencyDatabaseModel("UAH", 0.027107671m);
        var to = new CurrencyDatabaseModel("GBP", 1.1829374m);
        const decimal amount = 1456.69m;

        _currencyRepository.Setup(r => r.FindByCodeAsync(fromCurrency))
            .Returns(Task.FromResult<CurrencyDatabaseModel?>(from));
        _currencyRepository.Setup(r => r.FindByCodeAsync(toCurrency))
            .Returns(Task.FromResult<CurrencyDatabaseModel?>(to));

        // Act
        var result = await _currencyExchanger.Exchange(fromCurrency, toCurrency, amount);

        // Assert
        Assert.That(result, Is.EqualTo(33.380865m));
    }

    [Test]
    public async Task ToLowerExchangeRate()
    {
        // Arrange
        const string fromCurrency = "UAH";
        const string toCurrency = "USD";
        var from = new CurrencyDatabaseModel("USD", 1);
        var to = new CurrencyDatabaseModel("UAH", 0.0270803m);
        const decimal amount = 100;

        _currencyRepository.Setup(r => r.FindByCodeAsync(fromCurrency))
            .Returns(Task.FromResult<CurrencyDatabaseModel?>(from));
        _currencyRepository.Setup(r => r.FindByCodeAsync(toCurrency))
            .Returns(Task.FromResult<CurrencyDatabaseModel?>(to));

        // Act
        var result = await _currencyExchanger.Exchange(fromCurrency, toCurrency, amount);

        // Assert
        Assert.That(result, Is.EqualTo(3692.721277));
    }
}
