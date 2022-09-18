using System.Text;
using Microsoft.Extensions.Logging;
using Moq;
using Service.PersonalData.Abstractions;
using Service.PersonalData.Models;
using Shared.Grpc;
using Shared.Grpc.PersonalData.Contracts;
using Shared.Grpc.PersonalData.Models;

namespace Service.PersonalData.Tests;

public class PersonalDataGrpcServiceTest
{
    private Mock<IPersonalDataService> _personalDataService = null!;
    private Mock<IKycScansService> _kycScansService = null!;
    private Mock<ILogger<PersonalDataGrpcService>> _logger = null!;
    private PersonalDataGrpcService _personalDataGrpcService = null!;

    [SetUp]
    public void Setup()
    {
        _personalDataService = new Mock<IPersonalDataService>();
        _kycScansService = new Mock<IKycScansService>();
        _logger = new Mock<ILogger<PersonalDataGrpcService>>();

        _personalDataGrpcService =
            new PersonalDataGrpcService(_personalDataService.Object, _kycScansService.Object, _logger.Object);
    }

    [Test]
    public async Task ApplyPersonalDataSuccessful()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var personalDataCreate = new PersonalDataCreateGrpcModel { UserId = userId };
        var request = new ApplyPersonalDataGrpcRequest
        {
            PersonalData = personalDataCreate, KycScans = new List<KycScanCreateGrpcModel>()
        };
        var personalDataId = Guid.NewGuid();
        const string firstName = "first name";
        const string lastName = "last name";
        const string country = "country";
        const string city = "city";
        var personalDataDatabaseModel = new PersonalDataDatabaseModel
        {
            Id = personalDataId,
            UserId = userId,
            FirstName = firstName,
            LastName = lastName,
            Country = country,
            City = city,
            Status = PersonalDataStatus.Declined
        };

        _personalDataService.Setup(s => s.FindByUserId(userId))
            .Returns(Task.FromResult<PersonalDataDatabaseModel?>(personalDataDatabaseModel));
        _personalDataService.Setup(s => s.ApplyPersonalDataAsync(request))
            .Returns(Task.FromResult(personalDataDatabaseModel));

        // Act
        var response = await _personalDataGrpcService.ApplyPersonalDataAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.Ok));
            Assert.That(response.PersonalData?.Id, Is.EqualTo(personalDataId));
            Assert.That(response.PersonalData?.UserId, Is.EqualTo(userId));
            Assert.That(response.PersonalData?.FirstName, Is.EqualTo(firstName));
            Assert.That(response.PersonalData?.LastName, Is.EqualTo(lastName));
            Assert.That(response.PersonalData?.Country, Is.EqualTo(country));
            Assert.That(response.PersonalData?.City, Is.EqualTo(city));
        });
        _personalDataService.Verify(s => s.ApplyPersonalDataAsync(request), Times.Once);
    }

    [Test]
    public async Task ApplyPersonalDataAlreadyInPending()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var personalDataCreate = new PersonalDataCreateGrpcModel { UserId = userId };
        var request = new ApplyPersonalDataGrpcRequest
        {
            PersonalData = personalDataCreate,
            KycScans = new List<KycScanCreateGrpcModel>()
        };
        var personalDataId = Guid.NewGuid();
        const string firstName = "first name";
        const string lastName = "last name";
        const string country = "country";
        const string city = "city";
        var personalDataDatabaseModel = new PersonalDataDatabaseModel
        {
            Id = personalDataId,
            UserId = userId,
            FirstName = firstName,
            LastName = lastName,
            Country = country,
            City = city,
            Status = PersonalDataStatus.PendingApproval
        };

        _personalDataService.Setup(s => s.FindByUserId(userId))
            .Returns(Task.FromResult<PersonalDataDatabaseModel?>(personalDataDatabaseModel));

        // Act
        var response = await _personalDataGrpcService.ApplyPersonalDataAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.AlreadyInProcess));
            Assert.That(response.PersonalData?.Id, Is.EqualTo(personalDataId));
            Assert.That(response.PersonalData?.UserId, Is.EqualTo(userId));
            Assert.That(response.PersonalData?.FirstName, Is.EqualTo(firstName));
            Assert.That(response.PersonalData?.LastName, Is.EqualTo(lastName));
            Assert.That(response.PersonalData?.Country, Is.EqualTo(country));
            Assert.That(response.PersonalData?.City, Is.EqualTo(city));
        });
        _personalDataService.Verify(s => s.ApplyPersonalDataAsync(request), Times.Never);
    }

    [Test]
    public async Task ApplyPersonalDataAlreadyApproved()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var personalDataCreate = new PersonalDataCreateGrpcModel { UserId = userId };
        var request = new ApplyPersonalDataGrpcRequest
        {
            PersonalData = personalDataCreate,
            KycScans = new List<KycScanCreateGrpcModel>()
        };
        var personalDataDatabaseModel = new PersonalDataDatabaseModel
        {
            UserId = userId,
            Status = PersonalDataStatus.Approved
        };

        _personalDataService.Setup(s => s.FindByUserId(userId))
            .Returns(Task.FromResult<PersonalDataDatabaseModel?>(personalDataDatabaseModel));

        // Act
        var response = await _personalDataGrpcService.ApplyPersonalDataAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.AlreadyApproved));
            Assert.That(response.PersonalData, Is.Null);
        });
        _personalDataService.Verify(s => s.ApplyPersonalDataAsync(request), Times.Never);
    }

    [Test]
    public async Task ListAllUnapprovedPersonalDataSuccessful()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var personalDataId = Guid.NewGuid();
        const string firstName = "first name";
        const string lastName = "last name";
        const string country = "country";
        const string city = "city";
        var personalDataDatabaseModel = new PersonalDataDatabaseModel
        {
            Id = personalDataId,
            UserId = userId,
            FirstName = firstName,
            LastName = lastName,
            Country = country,
            City = city,
            Status = PersonalDataStatus.PendingApproval
        };

        _personalDataService.Setup(s => s.GetUnapprovedAsync())
            .Returns(Task.FromResult<IEnumerable<PersonalDataDatabaseModel>>(new[] { personalDataDatabaseModel }));

        // Act
        var response = await _personalDataGrpcService.ListAllUnapprovedPersonalDataAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.Ok));
            Assert.That(response.PersonalDataList[0].Id, Is.EqualTo(personalDataId));
            Assert.That(response.PersonalDataList[0].UserId, Is.EqualTo(userId));
            Assert.That(response.PersonalDataList[0].FirstName, Is.EqualTo(firstName));
            Assert.That(response.PersonalDataList[0].LastName, Is.EqualTo(lastName));
            Assert.That(response.PersonalDataList[0].Country, Is.EqualTo(country));
            Assert.That(response.PersonalDataList[0].City, Is.EqualTo(city));
        });
    }

    [Test]
    public async Task GetByIdSuccessful()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var personalDataId = Guid.NewGuid();
        const string firstName = "first name";
        const string lastName = "last name";
        const string country = "country";
        const string city = "city";
        var personalDataDatabaseModel = new PersonalDataDatabaseModel
        {
            Id = personalDataId,
            UserId = userId,
            FirstName = firstName,
            LastName = lastName,
            Country = country,
            City = city,
            Status = PersonalDataStatus.PendingApproval
        };
        var fileName = Guid.NewGuid();
        const string fileExtension = ".png";
        const string originalName = "test";
        const string contentType = "image/png";
        var content = Encoding.ASCII.GetBytes("some image content");
        var kycScanGrpcModel = new KycScanGrpcModel
        {
            FileName = fileName,
            PersonalDataId = personalDataId,
            FileExtension = fileExtension,
            OriginalName = originalName,
            ContentType = contentType,
            Content = content
        };
        var request = new GetPersonalDataByIdGrpcRequest { PersonalDataId = personalDataId };

        _personalDataService.Setup(s => s.GetByIdAsync(personalDataId))
            .Returns(Task.FromResult(personalDataDatabaseModel));
        _kycScansService.Setup(s => s.GetAllByPersonalData(personalDataDatabaseModel))
            .Returns(Task.FromResult<IEnumerable<KycScanGrpcModel>>(new[] { kycScanGrpcModel }));

        // Act
        var response = await _personalDataGrpcService.GetByIdAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.Ok));
            Assert.That(response.PersonalData?.Id, Is.EqualTo(personalDataId));
            Assert.That(response.PersonalData?.UserId, Is.EqualTo(userId));
            Assert.That(response.PersonalData?.FirstName, Is.EqualTo(firstName));
            Assert.That(response.PersonalData?.LastName, Is.EqualTo(lastName));
            Assert.That(response.PersonalData?.Country, Is.EqualTo(country));
            Assert.That(response.PersonalData?.City, Is.EqualTo(city));
            Assert.That(response.KycScansIds, Is.EqualTo(new[] { fileName }));
        });
    }

    [Test]
    public async Task GetByUserIdSuccessful()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var personalDataId = Guid.NewGuid();
        const string firstName = "first name";
        const string lastName = "last name";
        const string country = "country";
        const string city = "city";
        var personalDataDatabaseModel = new PersonalDataDatabaseModel
        {
            Id = personalDataId,
            UserId = userId,
            FirstName = firstName,
            LastName = lastName,
            Country = country,
            City = city,
            Status = PersonalDataStatus.PendingApproval
        };
        var request = new GetByUserIdPersonalDataByIdGrpcRequest { UserId = userId };

        _personalDataService.Setup(s => s.FindByUserId(userId))
            .Returns(Task.FromResult<PersonalDataDatabaseModel?>(personalDataDatabaseModel));

        // Act
        var response = await _personalDataGrpcService.GetByUserIdAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.Ok));
            Assert.That(response.PersonalData?.Id, Is.EqualTo(personalDataId));
            Assert.That(response.PersonalData?.UserId, Is.EqualTo(userId));
            Assert.That(response.PersonalData?.FirstName, Is.EqualTo(firstName));
            Assert.That(response.PersonalData?.LastName, Is.EqualTo(lastName));
            Assert.That(response.PersonalData?.Country, Is.EqualTo(country));
            Assert.That(response.PersonalData?.City, Is.EqualTo(city));
        });
    }

    [Test]
    public async Task ApprovePersonalDataSuccessful()
    {
        // Arrange
        var personalDataId = Guid.NewGuid();
        var request = new ApprovePersonalDataGrpcRequest { PersonalDataId = personalDataId };
        var personalDataDatabaseModel = new PersonalDataDatabaseModel();

        _personalDataService.Setup(s => s.GetByIdAsync(personalDataId))
            .Returns(Task.FromResult(personalDataDatabaseModel));
        _personalDataService.Setup(s => s.Approve(personalDataDatabaseModel))
            .Returns(Task.FromResult(true));

        // Act
        var response = await _personalDataGrpcService.ApprovePersonalDataAsync(request);

        // Assert
        Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.Ok));
    }

    [Test]
    public async Task ApprovePersonalDataAlreadyChanged()
    {
        // Arrange
        var personalDataId = Guid.NewGuid();
        var request = new ApprovePersonalDataGrpcRequest { PersonalDataId = personalDataId };
        var personalDataDatabaseModel = new PersonalDataDatabaseModel();

        _personalDataService.Setup(s => s.GetByIdAsync(personalDataId))
            .Returns(Task.FromResult(personalDataDatabaseModel));
        _personalDataService.Setup(s => s.Approve(personalDataDatabaseModel))
            .Returns(Task.FromResult(false));

        // Act
        var response = await _personalDataGrpcService.ApprovePersonalDataAsync(request);

        // Assert
        Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.AlreadyChanged));
    }

    [Test]
    public async Task DeclinePersonalDataSuccessful()
    {
        // Arrange
        var personalDataId = Guid.NewGuid();
        var request = new DeclinePersonalDataGrpcRequest { PersonalDataId = personalDataId };
        var personalDataDatabaseModel = new PersonalDataDatabaseModel();

        _personalDataService.Setup(s => s.GetByIdAsync(personalDataId))
            .Returns(Task.FromResult(personalDataDatabaseModel));
        _personalDataService.Setup(s => s.Decline(personalDataDatabaseModel))
            .Returns(Task.FromResult(true));

        // Act
        var response = await _personalDataGrpcService.DeclinePersonalDataAsync(request);

        // Assert
        Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.Ok));
    }

    [Test]
    public async Task DeclinePersonalDataAlreadyChanged()
    {
        // Arrange
        var personalDataId = Guid.NewGuid();
        var request = new DeclinePersonalDataGrpcRequest { PersonalDataId = personalDataId };
        var personalDataDatabaseModel = new PersonalDataDatabaseModel();

        _personalDataService.Setup(s => s.GetByIdAsync(personalDataId))
            .Returns(Task.FromResult(personalDataDatabaseModel));
        _personalDataService.Setup(s => s.Decline(personalDataDatabaseModel))
            .Returns(Task.FromResult(false));

        // Act
        var response = await _personalDataGrpcService.DeclinePersonalDataAsync(request);

        // Assert
        Assert.That(response.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.AlreadyChanged));
    }
}
