using Gateway.Root.PersonalData.Application;
using Moq;
using Shared.Grpc;
using Shared.Grpc.PersonalData;
using Shared.Grpc.PersonalData.Contracts;
using Shared.Grpc.PersonalData.Models;

namespace Gateway.Root.Tests.PersonalData;

public class PersonalDataManagementServiceTest
{
    private Mock<IPersonalDataGrpcService> _personalDataGrpcService = null!;
    private PersonalDataManagementService _personalDataManagement = null!;

    [SetUp]
    public void Setup()
    {
        _personalDataGrpcService = new Mock<IPersonalDataGrpcService>();
        _personalDataManagement = new PersonalDataManagementService(_personalDataGrpcService.Object);
    }

    [Test]
    public async Task ListAllUnapprovedSuccessful()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        const string firstName = "first name";
        const string lastName = "last name";
        const string country = "country";
        const string city = "city";
        var personalDataGrpcModel = new PersonalDataGrpcModel
        {
            Id = id,
            UserId = userId,
            FirstName = firstName,
            LastName = lastName,
            Country = country,
            City = city
        };
        var response = new ListAllUnapprovedPersonalDataGrpcResponse
        {
            GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Ok },
            PersonalDataList = new List<PersonalDataGrpcModel> { personalDataGrpcModel }
        };

        _personalDataGrpcService.Setup(s => s.ListAllUnapprovedPersonalDataAsync())
            .Returns(Task.FromResult(response));

        // Act
        var result = await _personalDataManagement.ListAllUnapproved();

        // Assert
        var dtos = result.Content.ToList();
        Assert.Multiple(() =>
        {
            Assert.That(result.GrpcStatus.Status, Is.EqualTo(GrpcResponseStatus.Ok));
            Assert.That(dtos[0].Id, Is.EqualTo(id));
            Assert.That(dtos[0].UserId, Is.EqualTo(userId));
            Assert.That(dtos[0].FirstName, Is.EqualTo(firstName));
            Assert.That(dtos[0].LastName, Is.EqualTo(lastName));
            Assert.That(dtos[0].Country, Is.EqualTo(country));
            Assert.That(dtos[0].City, Is.EqualTo(city));
        });
    }

    [Test]
    public async Task GetPersonalDataByIdSuccessful()
    {
        // Arrange
        var personalDataId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        const string firstName = "first name";
        const string lastName = "last name";
        const string country = "country";
        const string city = "city";
        var personalDataGrpcModel = new PersonalDataGrpcModel
        {
            Id = personalDataId,
            UserId = userId,
            FirstName = firstName,
            LastName = lastName,
            Country = country,
            City = city
        };
        var kycScanId = Guid.NewGuid();
        var response = new GetPersonalDataByIdGrpcResponse
        {
            GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Ok },
            PersonalData = personalDataGrpcModel,
            KycScansIds = new List<Guid> { kycScanId }
        };

        _personalDataGrpcService.Setup(s => s.GetByIdAsync(It.Is<GetPersonalDataByIdGrpcRequest>(r =>
            r.PersonalDataId == personalDataId))
        ).Returns(Task.FromResult(response));

        // Act
        var result = await _personalDataManagement.GetPersonalDataById(personalDataId);

        // Assert
        Assert.That(result.GrpcStatus.Status, Is.EqualTo(GrpcResponseStatus.Ok));
        Assert.That(result.Content?.Id, Is.EqualTo(personalDataId));
        Assert.That(result.Content?.UserId, Is.EqualTo(userId));
        Assert.That(result.Content?.FirstName, Is.EqualTo(firstName));
        Assert.That(result.Content?.LastName, Is.EqualTo(lastName));
        Assert.That(result.Content?.Country, Is.EqualTo(country));
        Assert.That(result.Content?.City, Is.EqualTo(city));
    }

    [Test]
    public async Task ApproveSuccessful()
    {
        // Arrange
        var personalDataId = Guid.NewGuid();
        var response = new ApprovePersonalDataGrpcResponse
        {
            GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Ok }
        };

        _personalDataGrpcService.Setup(s => s.ApprovePersonalDataAsync(It.Is<ApprovePersonalDataGrpcRequest>(r =>
            r.PersonalDataId == personalDataId))
        ).Returns(Task.FromResult(response));


        // Act
        var result = await _personalDataManagement.ApproveAsync(personalDataId);

        // Assert
        Assert.That(result.GrpcStatus.Status, Is.EqualTo(GrpcResponseStatus.Ok));
    }

    [Test]
    public async Task DeclineSuccessful()
    {
        // Arrange
        var personalDataId = Guid.NewGuid();
        var response = new DeclinePersonalDataGrpcResponse
        {
            GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Ok }
        };

        _personalDataGrpcService.Setup(s => s.DeclinePersonalDataAsync(It.Is<DeclinePersonalDataGrpcRequest>(r =>
            r.PersonalDataId == personalDataId))
        ).Returns(Task.FromResult(response));


        // Act
        var result = await _personalDataManagement.DeclineAsync(personalDataId);

        // Assert
        Assert.That(result.GrpcStatus.Status, Is.EqualTo(GrpcResponseStatus.Ok));
    }
}
