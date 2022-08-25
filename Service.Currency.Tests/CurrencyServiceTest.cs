using Service.Currency.Model;
using Service.Currency.Service;

namespace Service.Currency.Tests;

public class Tests
{
    private CurrencyService _currencyService = null!;

    [SetUp]
    public void Setup()
    {
        _currencyService = new CurrencyService();
    }

    [Test]
    public void TestUahToUsd()
    {
        // Arrange
        var from = new CurrencyDatabaseModel("UAH", 0.0271m);
        var to = new CurrencyDatabaseModel("USD", 1m);
        const decimal amount = 1456.69m;

        // Act
        var result = _currencyService.Exchange(from, to, amount);

        // Assert
        Assert.That(result, Is.EqualTo(39.476299m));
    }

    [Test]
    public void TestUahToGbp()
    {
        // Arrange
        var from = new CurrencyDatabaseModel("UAH", 0.027107671m);
        var to = new CurrencyDatabaseModel("GBP", 1.1829374m);
        const decimal amount = 1456.69m;

        // Act
        var result = _currencyService.Exchange(from, to, amount);

        // Assert
        Assert.That(result, Is.EqualTo(33.380865m));
    }
}
