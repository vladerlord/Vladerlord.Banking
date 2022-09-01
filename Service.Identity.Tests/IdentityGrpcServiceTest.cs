using System.Security.Claims;
using MassTransit;
using Moq;
using Npgsql;
using Service.Identity.Abstractions;
using Service.Identity.Models;
using Service.Identity.Services;
using Shared.Abstractions;
using Shared.Grpc;
using Shared.Grpc.Identity.Contracts;

namespace Service.Identity.Tests;

public class IdentityGrpcServiceTest
{
    private Mock<IUserRepository> _userRepository = null!;
    private Mock<IConfirmationLinkRepository> _confirmationLinkRepository = null!;
    private TokenAuthService _tokenAuthService = null!;
    private Mock<IPublishEndpoint> _publishEndpoint = null!;
    private HashService _hashService = null!;
    private IdentityGrpcService _identityGrpcService = null!;

    [SetUp]
    public void Setup()
    {
        const string secret =
            "XCAP05H6LoKvbRRa/QkqLNMI7cOHguaRyHzyg7n5qEkGjQmtBhz4SzYh4Fqwjyi3KJHlSXKPwVu2+bXr6CtpgQ==";
        const int expireInMinutes = 60;

        _userRepository = new Mock<IUserRepository>();
        _confirmationLinkRepository = new Mock<IConfirmationLinkRepository>();
        _tokenAuthService = new TokenAuthService(secret, expireInMinutes);
        _publishEndpoint = new Mock<IPublishEndpoint>();
        _hashService = new HashService();

        _identityGrpcService = new IdentityGrpcService(
            _userRepository.Object,
            _confirmationLinkRepository.Object,
            _tokenAuthService,
            _publishEndpoint.Object,
            _hashService
        );
    }

    [Test]
    public async Task LoginSuccessful()
    {
        // Arrange
        const string email = "test@gmail.com";
        var user = new UserDatabaseModel(Guid.NewGuid(), email, _hashService.Hash("password"), UserStatus.Created);
        var userAsGrpcModel = user.ToGrpcModel();

        var request = new LoginGrpcRequest
        {
            Email = email,
            Password = "password"
        };

        _userRepository.Setup(r => r.FindByEmailAsync(email))
            .Returns(Task.FromResult<UserDatabaseModel?>(user));

        // Act
        var result = await _identityGrpcService.LoginAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.Ok));
            Assert.That(result.Jwt, Is.Not.Null);
            Assert.That(_tokenAuthService.VerifyToken(result.Jwt ?? ""), Is.Not.Null);
            Assert.That(userAsGrpcModel.Id, Is.EqualTo(result.UserModel?.Id));
            Assert.That(userAsGrpcModel.Email, Is.EqualTo(result.UserModel?.Email));
        });
    }

    [Test]
    public async Task LoginUserNotFound()
    {
        // Act
        const string email = "test@gmail.com";
        var request = new LoginGrpcRequest
        {
            Email = email,
            Password = "password"
        };

        _userRepository.Setup(r => r.FindByEmailAsync(email))
            .Returns(Task.FromResult<UserDatabaseModel?>(null));

        // Act
        var result = await _identityGrpcService.LoginAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.NotFound));
            Assert.That(result.Jwt, Is.Null);
            Assert.That(result.UserModel, Is.Null);
        });
    }

    [Test]
    public async Task LoginWrongPassword()
    {
        // Act
        const string email = "test@gmail.com";
        var user = new UserDatabaseModel(Guid.NewGuid(), email, _hashService.Hash("password"), UserStatus.Created);
        var request = new LoginGrpcRequest
        {
            Email = email,
            Password = "password1"
        };

        _userRepository.Setup(r => r.FindByEmailAsync(email))
            .Returns(Task.FromResult<UserDatabaseModel?>(user));

        // Act
        var result = await _identityGrpcService.LoginAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.NotFound));
            Assert.That(result.Jwt, Is.Null);
            Assert.That(result.UserModel, Is.Null);
        });
    }

    [Test]
    public async Task RegisterSuccessful()
    {
        // Act
        var id = Guid.NewGuid();
        const string email = "test@gmail.com";
        var user = new UserDatabaseModel(id, email, _hashService.Hash("password"), UserStatus.Created);
        var request = new RegisterUserGrpcRequest
        {
            Email = email,
            Password = "password1",
            ConfirmationCode = "qwe",
            RegisterConfirmationUrl = "http://someurl.com/confirm/qwe"
        };

        _userRepository.Setup(r => r.CreateAsync(It.IsAny<UserDatabaseModel>()))
            .Returns(Task.FromResult(user));

        // Act
        var result = await _identityGrpcService.RegisterAsync(request);

        // Assert
        _confirmationLinkRepository.Verify(r => r.CreateAsync(It.IsAny<ConfirmationLinkDatabaseModel>()), Times.Once);
        Assert.Multiple(() =>
        {
            Assert.That(result.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.Ok));
            Assert.That(result.UserModel?.Id, Is.EqualTo(id));
            Assert.That(result.UserModel?.Email, Is.EqualTo(email));
        });
    }

    [Test]
    public async Task RegisterEmailIsTaken()
    {
        // Arrange
        const string email = "test@gmail.com";
        var request = new RegisterUserGrpcRequest
        {
            Email = email,
            Password = "password1",
            ConfirmationCode = "qwe",
            RegisterConfirmationUrl = "http://someurl.com/confirm/qwe"
        };

        _userRepository.Setup(r => r.CreateAsync(It.IsAny<UserDatabaseModel>()))
            .Throws(new PostgresException("message", "severity", "invariantSeverity",
                PostgresErrorCodes.UniqueViolation, constraintName: "users_email"));

        // Act
        var result = await _identityGrpcService.RegisterAsync(request);

        // Assert
        Assert.That(result.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.UserAlreadyExist));
    }

    [Test]
    public async Task VerifyTokenSuccessful()
    {
        // Arrange
        const string email = "test@gmail.com";
        var user = new UserDatabaseModel(Guid.NewGuid(), email, _hashService.Hash("password"), UserStatus.Created);
        var request = new VerifyTokenGrpcRequest
        {
            JwtToken = _tokenAuthService.GenerateToken(user)
        };

        // Act
        var result = await _identityGrpcService.VerifyTokenAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Status, Is.EqualTo(GrpcResponseStatus.Ok));
            Assert.That(result.Claims, Does.Contain(new KeyValuePair<string, string>(ClaimTypes.Email, email)));
        });
    }

    [Test]
    public async Task VerifyTokenError()
    {
        // Arrange
        var request = new VerifyTokenGrpcRequest
        {
            JwtToken = "wrong jwt"
        };

        // Act
        var result = await _identityGrpcService.VerifyTokenAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Status, Is.EqualTo(GrpcResponseStatus.Invalid));
            Assert.That(result.Claims, Is.Null);
        });
    }

    [Test]
    public async Task RegisterConfirmationSuccessful()
    {
        // Arrange
        const string code = "qwe";
        var userId = Guid.NewGuid();
        var confirmationLinkId = Guid.NewGuid();
        var userModel = new UserDatabaseModel(userId, "email", "password", UserStatus.Created);
        var confirmationModel = new ConfirmationLinkDatabaseModel
        {
            Id = confirmationLinkId,
            Type = ConfirmationLinkType.RegistrationConfirmation,
            ConfirmationCode = code,
            UserId = userId
        };
        var request = new RegisterConfirmationGrpcRequest
        {
            ConfirmationCode = code
        };

        _confirmationLinkRepository
            .Setup(r => r.FindByConfirmationCodeAsync(code))
            .Returns(Task.FromResult<ConfirmationLinkDatabaseModel?>(confirmationModel));
        _userRepository
            .Setup(r => r.FindByIdAsync(userId))
            .Returns(Task.FromResult<UserDatabaseModel?>(userModel));

        // Act
        var result = await _identityGrpcService.RegisterConfirmationAsync(request);

        // Assert
        Assert.That(result.Status, Is.EqualTo(GrpcResponseStatus.Ok));
        _userRepository.Verify(r => r.UpdateStatusAsync(userId, UserStatus.Confirmed), Times.Once);
        _confirmationLinkRepository.Verify(r => r.DeleteByIdAsync(confirmationLinkId), Times.Once);
    }

    [Test]
    public async Task RegisterConfirmationLinkNotFound()
    {
        // Arrange
        const string code = "qwe";
        var request = new RegisterConfirmationGrpcRequest
        {
            ConfirmationCode = code
        };

        _confirmationLinkRepository
            .Setup(r => r.FindByConfirmationCodeAsync(code))
            .Returns(Task.FromResult<ConfirmationLinkDatabaseModel?>(null));

        // Act
        var result = await _identityGrpcService.RegisterConfirmationAsync(request);

        // Assert
        Assert.That(result.Status, Is.EqualTo(GrpcResponseStatus.NotFound));
        _userRepository.Verify(r => r.UpdateStatusAsync(It.IsAny<Guid>(), UserStatus.Confirmed), Times.Never);
        _confirmationLinkRepository.Verify(r => r.DeleteByIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Test]
    public async Task RegisterConfirmationUserNotFound()
    {
        // Arrange
        const string code = "qwe";
        var userId = Guid.NewGuid();
        var confirmationLinkId = Guid.NewGuid();
        var confirmationModel = new ConfirmationLinkDatabaseModel
        {
            Id = confirmationLinkId,
            Type = ConfirmationLinkType.RegistrationConfirmation,
            ConfirmationCode = code,
            UserId = userId
        };
        var request = new RegisterConfirmationGrpcRequest
        {
            ConfirmationCode = code
        };

        _confirmationLinkRepository
            .Setup(r => r.FindByConfirmationCodeAsync(code))
            .Returns(Task.FromResult<ConfirmationLinkDatabaseModel?>(confirmationModel));
        _userRepository
            .Setup(r => r.FindByIdAsync(userId))
            .Returns(Task.FromResult<UserDatabaseModel?>(null));

        // Act
        var result = await _identityGrpcService.RegisterConfirmationAsync(request);

        // Assert
        Assert.That(result.Status, Is.EqualTo(GrpcResponseStatus.NotFound));
        _userRepository.Verify(r => r.UpdateStatusAsync(userId, UserStatus.Confirmed), Times.Never);
        _confirmationLinkRepository.Verify(r => r.DeleteByIdAsync(confirmationLinkId), Times.Never);
    }
}
