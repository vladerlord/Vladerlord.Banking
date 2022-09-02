using Microsoft.Extensions.Logging;
using Moq;
using Service.BankAccount.Abstraction;
using Service.BankAccount.Model;
using Shared.Grpc;
using Shared.Grpc.BankAccount.Contract;
using Shared.Grpc.Currency;
using Shared.Grpc.Currency.Contract;

namespace Service.BankAccount.Tests;

public class BankAccountGrpcServiceTest
{
    private Mock<IBankAccountRepository> _bankAccountRepository = null!;
    private Mock<ICurrencyGrpcService> _currencyGrpcService = null!;
    private Mock<ILogger<BankAccountGrpcService>> _logger = null!;
    private BankAccountGrpcService _bankAccountGrpcService = null!;

    [SetUp]
    public void Setup()
    {
        _bankAccountRepository = new Mock<IBankAccountRepository>();
        _currencyGrpcService = new Mock<ICurrencyGrpcService>();
        _logger = new Mock<ILogger<BankAccountGrpcService>>();

        _bankAccountGrpcService = new BankAccountGrpcService(
            _bankAccountRepository.Object,
            _currencyGrpcService.Object,
            _logger.Object
        );
    }

    [Test]
    public async Task CreateSuccessful()
    {
        // Arrange
        var id = Guid.NewGuid();
        var personalDataId = Guid.NewGuid();
        const string currency = "USD";
        const string expireAt = "24.03.2024";
        var request = new CreateBankAccountGrpcRequest
        {
            PersonalDataId = personalDataId,
            CurrencyCode = currency,
            ExpireAt = expireAt
        };
        var bankAccountModel = new BankAccountDatabaseModel(id, personalDataId, currency, 0m, expireAt);

        _bankAccountRepository
            .Setup(r => r.CreateAsync(It.IsAny<BankAccountDatabaseModel>()))
            .Returns(Task.FromResult(bankAccountModel));

        // Act
        var response = await _bankAccountGrpcService.CreateAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.Ok));
            Assert.That(response.BankAccount?.Id, Is.EqualTo(id));
            Assert.That(response.BankAccount?.PersonalDataId, Is.EqualTo(personalDataId));
            Assert.That(response.BankAccount?.CurrencyCode, Is.EqualTo(currency));
            Assert.That(response.BankAccount?.Balance, Is.EqualTo(0m));
            Assert.That(response.BankAccount?.ExpireAt, Is.EqualTo(expireAt));
        });
    }

    [Test]
    public async Task CreateError()
    {
        // Arrange
        var personalDataId = Guid.NewGuid();
        const string currency = "USD";
        const string expireAt = "24.03.2024";
        var request = new CreateBankAccountGrpcRequest
        {
            PersonalDataId = personalDataId,
            CurrencyCode = currency,
            ExpireAt = expireAt
        };

        _bankAccountRepository
            .Setup(r => r.CreateAsync(It.IsAny<BankAccountDatabaseModel>()))
            .Throws(new Exception("Some exception"));

        // Act
        var response = await _bankAccountGrpcService.CreateAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.Error));
            Assert.That(response.BankAccount, Is.Null);
        });
    }

    [Test]
    public async Task GetByIdSuccessful()
    {
        // Arrange
        var id = Guid.NewGuid();
        var personalDataId = Guid.NewGuid();
        const string currency = "USD";
        const string expireAt = "24.03.2024";
        var bankAccountModel = new BankAccountDatabaseModel(id, personalDataId, currency, 0m, expireAt);
        var request = new GetBankAccountByIdGrpcRequest
        {
            BankAccountId = id,
        };

        _bankAccountRepository
            .Setup(r => r.FindByIdAsync(id))
            .Returns(Task.FromResult<BankAccountDatabaseModel?>(bankAccountModel));

        // Act
        var response = await _bankAccountGrpcService.GetByIdAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.Ok));
            Assert.That(response.BankAccount?.Id, Is.EqualTo(id));
            Assert.That(response.BankAccount?.CurrencyCode, Is.EqualTo(currency));
            Assert.That(response.BankAccount?.Balance, Is.EqualTo(0m));
            Assert.That(response.BankAccount?.PersonalDataId, Is.EqualTo(personalDataId));
            Assert.That(response.BankAccount?.ExpireAt, Is.EqualTo(expireAt));
        });
    }

    [Test]
    public async Task GetByIdNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new GetBankAccountByIdGrpcRequest
        {
            BankAccountId = id,
        };

        _bankAccountRepository
            .Setup(r => r.FindByIdAsync(id))
            .Returns(Task.FromResult<BankAccountDatabaseModel?>(null));

        // Act
        var response = await _bankAccountGrpcService.GetByIdAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.NotFound));
            Assert.That(response.BankAccount, Is.Null);
        });
    }

    [Test]
    public async Task AddFundsSuccessfulWithoutCurrencyExchange()
    {
        // Arrange
        var bankAccountId = Guid.NewGuid();
        var personalDataId = Guid.NewGuid();
        const decimal amount = 10m;
        const string currency = "USD";
        const string expireAt = "24.03.2024";

        var bankAccount =
            new BankAccountDatabaseModel(bankAccountId, personalDataId, currency, 0m, expireAt);
        var request = new AddFundsGrpcRequest
        {
            BankAccountId = bankAccountId,
            Amount = amount,
            Currency = currency
        };

        _bankAccountRepository.Setup(r => r.FindByIdAsync(bankAccountId))
            .Returns(Task.FromResult<BankAccountDatabaseModel?>(bankAccount));

        // Act
        var response = await _bankAccountGrpcService.AddFundsAsync(request);

        // Assert
        _bankAccountRepository.Verify(r => r.AddToBalanceAsync(bankAccountId, amount), Times.Once);
        Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.Ok));
    }

    [Test]
    public async Task AddFundsSuccessfulWithCurrencyExchange()
    {
        // Arrange
        var bankAccountId = Guid.NewGuid();
        var personalDataId = Guid.NewGuid();
        const decimal amount = 10;
        const decimal exchangedAmount = 350;
        const string fromCurrency = "USD";
        const string toCurrency = "UAH";
        const string expireAt = "24.03.2024";
        var bankAccount =
            new BankAccountDatabaseModel(bankAccountId, personalDataId, toCurrency, 0m, expireAt);
        var request = new AddFundsGrpcRequest
        {
            BankAccountId = bankAccountId,
            Amount = amount,
            Currency = fromCurrency
        };
        var exchangeResponse = new ExchangeGrpcResponse
        {
            GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Ok },
            Amount = exchangedAmount
        };

        _bankAccountRepository.Setup(r => r.FindByIdAsync(bankAccountId))
            .Returns(Task.FromResult<BankAccountDatabaseModel?>(bankAccount));
        _currencyGrpcService
            .Setup(s => s.ExchangeAsync(It.Is<ExchangeGrpcRequest>(
                r => r.Amount == amount
                     && r.FromCurrencyCode == fromCurrency
                     && r.ToCurrencyCode == toCurrency
            )))
            .Returns(Task.FromResult(exchangeResponse));

        // Act
        var response = await _bankAccountGrpcService.AddFundsAsync(request);

        // Assert
        _bankAccountRepository.Verify(r => r.AddToBalanceAsync(bankAccountId, exchangedAmount), Times.Once);
        Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.Ok));
    }

    [Test]
    public async Task TakeFundsSuccessfulWithoutCurrencyExchange()
    {
        // Arrange
        var bankAccountId = Guid.NewGuid();
        var personalDataId = Guid.NewGuid();
        const decimal amount = 10m;
        const string currency = "USD";
        const string expireAt = "24.03.2024";
        var bankAccount =
            new BankAccountDatabaseModel(bankAccountId, personalDataId, currency, 0m, expireAt);
        var request = new TakeFundsGrpcRequest
        {
            BankAccountId = bankAccountId,
            Amount = amount,
            Currency = currency
        };

        _bankAccountRepository.Setup(r => r.FindByIdAsync(bankAccountId))
            .Returns(Task.FromResult<BankAccountDatabaseModel?>(bankAccount));

        // Act
        var response = await _bankAccountGrpcService.TakeFundsAsync(request);

        // Assert
        _bankAccountRepository.Verify(r => r.TakeFromBalanceAsync(bankAccountId, amount), Times.Once);
        Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.Ok));
    }

    [Test]
    public async Task TakeFundsSuccessfulWithCurrencyExchange()
    {
        // Arrange
        var bankAccountId = Guid.NewGuid();
        var personalDataId = Guid.NewGuid();
        const decimal amount = 10;
        const decimal exchangedAmount = 350;
        const string fromCurrency = "USD";
        const string toCurrency = "UAH";
        const string expireAt = "24.03.2024";
        var bankAccount =
            new BankAccountDatabaseModel(bankAccountId, personalDataId, toCurrency, 0m, expireAt);
        var request = new TakeFundsGrpcRequest
        {
            BankAccountId = bankAccountId,
            Amount = amount,
            Currency = fromCurrency
        };
        var exchangeResponse = new ExchangeGrpcResponse
        {
            GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Ok },
            Amount = exchangedAmount
        };

        _bankAccountRepository.Setup(r => r.FindByIdAsync(bankAccountId))
            .Returns(Task.FromResult<BankAccountDatabaseModel?>(bankAccount));
        _currencyGrpcService
            .Setup(s => s.ExchangeAsync(It.Is<ExchangeGrpcRequest>(
                r => r.Amount == amount
                     && r.FromCurrencyCode == fromCurrency
                     && r.ToCurrencyCode == toCurrency)
            ))
            .Returns(Task.FromResult(exchangeResponse));

        // Act
        var response = await _bankAccountGrpcService.TakeFundsAsync(request);

        // Assert
        _bankAccountRepository.Verify(r => r.TakeFromBalanceAsync(bankAccountId, exchangedAmount), Times.Once);
        Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.Ok));
    }

    [Test]
    public async Task TransferFundsSuccessfulWithoutCurrencyExchange()
    {
        // Arrange
        var fromBankAccountId = Guid.NewGuid();
        var fromPersonalDataId = Guid.NewGuid();
        const string fromCurrency = "USD";
        var toBankAccountId = Guid.NewGuid();
        var toPersonalDataId = Guid.NewGuid();
        const string toCurrency = "USD";
        const decimal amount = 10;
        const string expireAt = "24.03.2024";

        var request = new TransferFundsGrpcRequest
        {
            FromBankAccountId = fromBankAccountId,
            ToBankAccountId = toBankAccountId,
            Amount = amount
        };
        var fromBankAccount =
            new BankAccountDatabaseModel(fromBankAccountId, fromPersonalDataId, fromCurrency, 0, expireAt);
        var toBankAccount = new BankAccountDatabaseModel(toBankAccountId, toPersonalDataId, toCurrency, 0, expireAt);

        _bankAccountRepository.Setup(r => r.FindByIdAsync(fromBankAccountId))
            .Returns(Task.FromResult<BankAccountDatabaseModel?>(fromBankAccount));
        _bankAccountRepository.Setup(r => r.FindByIdAsync(toBankAccountId))
            .Returns(Task.FromResult<BankAccountDatabaseModel?>(toBankAccount));

        // Act
        var response = await _bankAccountGrpcService.TransferFundsAsync(request);

        // Assert
        _bankAccountRepository.Verify(r => r.TransferAsync(fromBankAccountId, toBankAccountId, amount, amount));
        Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.Ok));
    }

    [Test]
    public async Task TransferFundsSuccessfulWithCurrencyExchange()
    {
        // Arrange
        var fromBankAccountId = Guid.NewGuid();
        var fromPersonalDataId = Guid.NewGuid();
        const string fromCurrency = "USD";
        var toBankAccountId = Guid.NewGuid();
        var toPersonalDataId = Guid.NewGuid();
        const string toCurrency = "UAH";
        const decimal amount = 10;
        const decimal exchangedAmount = 350;
        const string expireAt = "24.03.2024";

        var request = new TransferFundsGrpcRequest
        {
            FromBankAccountId = fromBankAccountId,
            ToBankAccountId = toBankAccountId,
            Amount = amount
        };
        var exchangeResponse = new ExchangeGrpcResponse
        {
            GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Ok },
            Amount = exchangedAmount
        };
        var fromBankAccount =
            new BankAccountDatabaseModel(fromBankAccountId, fromPersonalDataId, fromCurrency, 0, expireAt);
        var toBankAccount = new BankAccountDatabaseModel(toBankAccountId, toPersonalDataId, toCurrency, 0, expireAt);

        _bankAccountRepository.Setup(r => r.FindByIdAsync(fromBankAccountId))
            .Returns(Task.FromResult<BankAccountDatabaseModel?>(fromBankAccount));
        _bankAccountRepository.Setup(r => r.FindByIdAsync(toBankAccountId))
            .Returns(Task.FromResult<BankAccountDatabaseModel?>(toBankAccount));
        _currencyGrpcService
            .Setup(s => s.ExchangeAsync(It.Is<ExchangeGrpcRequest>(
                r => r.Amount == amount
                     && r.FromCurrencyCode == fromCurrency
                     && r.ToCurrencyCode == toCurrency)
            ))
            .Returns(Task.FromResult(exchangeResponse));

        // Act
        var response = await _bankAccountGrpcService.TransferFundsAsync(request);

        // Assert
        _bankAccountRepository.Verify(r =>
            r.TransferAsync(fromBankAccountId, toBankAccountId, amount, exchangedAmount));
        Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.Ok));
    }
}
