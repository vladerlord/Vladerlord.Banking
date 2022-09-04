using Gateway.Root.Identity.Application;
using Moq;
using Shared.Grpc;
using Shared.Grpc.Identity;
using Shared.Grpc.Identity.Contracts;
using Shared.Grpc.Identity.Models;

namespace Gateway.Root.Tests.Identity;

public class UserServiceTest
{
    private Mock<IIdentityGrpcService> _identityGrpcService = null!;
    private UserService _userService = null!;

    [SetUp]
    public void Setup()
    {
        _identityGrpcService = new Mock<IIdentityGrpcService>();
        _userService = new UserService(_identityGrpcService.Object);
    }

    [Test]
    public async Task LoginSuccessful()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string email = "email";
        const string password = "password";
        const string jwt = "jwt";
        var grpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Ok };
        var response = new LoginGrpcResponse
        {
            GrpcResponse = grpcResponse,
            Jwt = jwt,
            UserModel = new UserGrpcModel
            {
                Id = userId,
                Email = email
            }
        };

        _identityGrpcService.Setup(s => s.LoginAsync(It.Is<LoginGrpcRequest>(request =>
            request.Email == email && request.Password == password
        ))).Returns(Task.FromResult(response));

        // Act
        var result = await _userService.LoginAsync(email, password);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.GrpcStatus, Is.EqualTo(grpcResponse));
            Assert.That(result.Content.Jwt, Is.EqualTo(jwt));
            Assert.That(result.Content.User?.Id, Is.EqualTo(userId));
            Assert.That(result.Content.User?.Email, Is.EqualTo(email));
        });
    }

    [Test]
    public async Task LoginWrongParameters()
    {
        // Arrange
        const string email = "email";
        const string password = "password";
        var grpcResponse = new GrpcResponse { Status = GrpcResponseStatus.NotFound };
        var response = new LoginGrpcResponse
        {
            GrpcResponse = grpcResponse,
            Jwt = null,
            UserModel = null
        };

        _identityGrpcService.Setup(s => s.LoginAsync(It.IsAny<LoginGrpcRequest>()))
            .Returns(Task.FromResult(response));

        // Act
        var result = await _userService.LoginAsync(email, password);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.GrpcStatus, Is.EqualTo(grpcResponse));
            Assert.That(result.Content.Jwt, Is.Null);
            Assert.That(result.Content.User, Is.Null);
        });
    }

    [Test]
    public async Task RegisterSuccessful()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string email = "email";
        const string password = "password";
        const string confirmationCode = "wqeqwe";
        const string confirmationUrl = "http://test.com/confirm/wqeqwe";

        var grpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Ok };
        var response = new RegisterUserGrpcResponse
        {
            GrpcResponse = grpcResponse,
            UserModel = new UserGrpcModel
            {
                Id = userId,
                Email = email
            }
        };

        _identityGrpcService.Setup(s => s.RegisterAsync(It.Is<RegisterUserGrpcRequest>(request =>
            request.Email == email &&
            request.Password == password &&
            request.ConfirmationCode == confirmationCode &&
            request.RegisterConfirmationUrl == confirmationUrl
        ))).Returns(Task.FromResult(response));

        // Act
        var result = await _userService.RegisterAsync(confirmationCode, confirmationUrl, email, password);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.GrpcStatus, Is.EqualTo(grpcResponse));
            Assert.That(result.Content?.Id, Is.EqualTo(userId));
            Assert.That(result.Content?.Email, Is.EqualTo(email));
        });
    }

    [Test]
    public async Task RegisterAlreadyExist()
    {
        // Arrange
        const string email = "email";
        const string password = "password";
        const string confirmationCode = "wqeqwe";
        const string confirmationUrl = "http://test.com/confirm/wqeqwe";

        var grpcResponse = new GrpcResponse { Status = GrpcResponseStatus.UserAlreadyExist };
        var response = new RegisterUserGrpcResponse { GrpcResponse = grpcResponse };

        _identityGrpcService.Setup(s => s.RegisterAsync(It.IsAny<RegisterUserGrpcRequest>()))
            .Returns(Task.FromResult(response));

        // Act
        var result = await _userService.RegisterAsync(confirmationCode, confirmationUrl, email, password);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.GrpcStatus, Is.EqualTo(grpcResponse));
            Assert.That(result.Content, Is.Null);
        });
    }
}
