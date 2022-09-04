using Moq;
using Service.PersonalData.Abstractions;
using Service.PersonalData.Models;
using Service.PersonalData.Services;
using Shared.Grpc.PersonalData.Contracts;
using Shared.Grpc.PersonalData.Models;

namespace Service.PersonalData.Tests;

public class PersonalDataServiceTest
{
    private Mock<IPersonalDataRepository> _personalDataRepository = null!;
    private Mock<IKycScansService> _kycScansService = null!;
    private EncryptionService _encryptionService = null!;
    private Mock<IPersonalDataEncryptionService> _personalDataEncryptionService = null!;
    private PersonalDataService _personalDataService = null!;

    [SetUp]
    public void Setup()
    {
        _personalDataRepository = new Mock<IPersonalDataRepository>();
        _kycScansService = new Mock<IKycScansService>();
        _encryptionService = new EncryptionService("MOn56M2W+dQ+RXxqD300GORRnRMOSK/nEJTpr2RzsmM=");
        _personalDataEncryptionService = new Mock<IPersonalDataEncryptionService>();

        _personalDataService = new PersonalDataService(
            _personalDataRepository.Object,
            _kycScansService.Object,
            _encryptionService,
            _personalDataEncryptionService.Object
        );
    }

    [Test]
    public async Task FindByUserIdSuccessfulDecryptIsCalled()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var personalDataDatabaseModel = new PersonalDataDatabaseModel
        {
            FirstName = "first name",
            LastName = "last name"
        };

        _personalDataRepository.Setup(r => r.FindByUserIdAsync(userId))
            .Returns(Task.FromResult<PersonalDataDatabaseModel?>(personalDataDatabaseModel));

        // Act
        var result = await _personalDataService.FindByUserId(userId);

        // Assert
        _personalDataEncryptionService.Verify(s => s.Decrypt(personalDataDatabaseModel), Times.Once);
        Assert.Multiple(() =>
        {
            Assert.That(result?.FirstName, Is.EqualTo("first name"));
            Assert.That(result?.LastName, Is.EqualTo("last name"));
        });
    }

    [Test]
    public async Task ApplyPersonalDataSuccessful()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string firstName = "first name";
        const string lastName = "last name";
        const string country = "country";
        const string city = "city";
        var personalDataCreate = new PersonalDataCreateGrpcModel
        {
            UserId = userId,
            FirstName = firstName,
            LastName = lastName,
            Country = country,
            City = city
        };
        var personalDataId = Guid.NewGuid();
        var personalDataDatabaseModel = new PersonalDataDatabaseModel
        {
            Id = personalDataId
        };
        const string fileName = "test";
        const string fileExtension = ".png";
        const string fileContentType = "image/png";

        var kycScanCreateGrpcModel = new KycScanCreateGrpcModel
        {
            FileName = fileName,
            FileExtension = fileExtension,
            ContentType = fileContentType,
            Content = Array.Empty<byte>()
        };
        var request = new ApplyPersonalDataGrpcRequest
        {
            PersonalData = personalDataCreate,
            KycScans = new List<KycScanCreateGrpcModel> { kycScanCreateGrpcModel }
        };

        _personalDataEncryptionService.Setup(s => s.Encrypt(It.IsAny<PersonalDataDatabaseModel>()))
            .Callback((PersonalDataDatabaseModel model) =>
            {
                model.FirstName = "encrypted first name";
                model.LastName = "encrypted last name";
            });
        _personalDataEncryptionService.Setup(s => s.Decrypt(It.IsAny<PersonalDataDatabaseModel>()))
            .Callback((PersonalDataDatabaseModel model) =>
            {
                model.FirstName = "decrypted first name";
                model.LastName = "decrypted last name";
            });
        _personalDataRepository.Setup(r => r.CreateOrUpdateAsync(It.IsAny<PersonalDataDatabaseModel>()))
            .Returns(Task.FromResult(personalDataDatabaseModel));

        // Act
        var result = await _personalDataService.ApplyPersonalDataAsync(request);

        // Assert
        _kycScansService.Verify(s => s.DeleteAllByPersonalDataId(personalDataId), Times.Once);
        _kycScansService.Verify(s => s.Create(request.KycScans, personalDataDatabaseModel), Times.Once);

        Assert.Multiple(() =>
        {
            Assert.That(result.FirstName, Is.EqualTo("decrypted first name"));
            Assert.That(result.LastName, Is.EqualTo("decrypted last name"));
        });
    }

    [Test]
    public async Task GetUnapprovedSuccessful()
    {
        // Arrange
        var personalDataDatabaseModel = new PersonalDataDatabaseModel
        {
            FirstName = "encrypted first name",
            LastName = "encrypted last name",
        };

        _personalDataRepository.Setup(r => r.GetUnapprovedAsync())
            .Returns(Task.FromResult(new List<PersonalDataDatabaseModel> { personalDataDatabaseModel }));
        _personalDataEncryptionService.Setup(s => s.Decrypt(personalDataDatabaseModel))
            .Callback((PersonalDataDatabaseModel model) =>
            {
                model.FirstName = "decrypted first name";
                model.LastName = "decrypted last name";
            });

        // Act
        var result = await _personalDataService.GetUnapprovedAsync();

        // Assert
        var resultList = result.ToList();

        Assert.Multiple(() =>
        {
            Assert.That(resultList[0].FirstName, Is.EqualTo("decrypted first name"));
            Assert.That(resultList[0].LastName, Is.EqualTo("decrypted last name"));
        });
    }

    [Test]
    public async Task GetByIdSuccessful()
    {
        // Arrange
        var personalDataId = Guid.NewGuid();
        var personalDataDatabaseModel = new PersonalDataDatabaseModel
        {
            FirstName = "encrypted first name",
            LastName = "encrypted last name",
        };

        _personalDataRepository.Setup(r => r.FindByIdAsync(personalDataId))
            .Returns(Task.FromResult<PersonalDataDatabaseModel?>(personalDataDatabaseModel));
        _personalDataEncryptionService.Setup(s => s.Decrypt(personalDataDatabaseModel))
            .Callback((PersonalDataDatabaseModel model) =>
            {
                model.FirstName = "decrypted first name";
                model.LastName = "decrypted last name";
            });

        // Act
        var result = await _personalDataService.GetByIdAsync(personalDataId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.FirstName, Is.EqualTo("decrypted first name"));
            Assert.That(result.LastName, Is.EqualTo("decrypted last name"));
        });
    }

    [Test]
    public async Task ApproveSuccessful()
    {
        // Arrange
        var personalDataId = Guid.NewGuid();
        var personalDataDatabaseModel = new PersonalDataDatabaseModel
        {
            Id = personalDataId,
            Status = PersonalDataStatus.PendingApproval
        };

        // Act
        var result = await _personalDataService.Approve(personalDataDatabaseModel);

        // Assert
        Assert.That(result, Is.True);
        _personalDataRepository.Verify(r => r.ChangeStatusAsync(personalDataId, PersonalDataStatus.Approved),
            Times.Once);
    }

    [Test]
    public async Task ApproveWrongStatus()
    {
        // Arrange
        var personalDataId = Guid.NewGuid();
        var personalDataDatabaseModel = new PersonalDataDatabaseModel
        {
            Id = personalDataId,
            Status = PersonalDataStatus.Declined
        };

        // Act
        var result = await _personalDataService.Approve(personalDataDatabaseModel);

        // Assert
        Assert.That(result, Is.False);
        _personalDataRepository.Verify(r => r.ChangeStatusAsync(personalDataId, PersonalDataStatus.Approved),
            Times.Never);
    }

    [Test]
    public async Task DeclineSuccessful()
    {
        // Arrange
        var personalDataId = Guid.NewGuid();
        var personalDataDatabaseModel = new PersonalDataDatabaseModel
        {
            Id = personalDataId,
            Status = PersonalDataStatus.PendingApproval
        };

        // Act
        var result = await _personalDataService.Decline(personalDataDatabaseModel);

        // Assert
        Assert.That(result, Is.True);
        _personalDataRepository.Verify(r => r.ChangeStatusAsync(personalDataId, PersonalDataStatus.Declined),
            Times.Once);
    }

    [Test]
    public async Task DeclineWrongStatus()
    {
        // Arrange
        var personalDataId = Guid.NewGuid();
        var personalDataDatabaseModel = new PersonalDataDatabaseModel
        {
            Id = personalDataId,
            Status = PersonalDataStatus.Declined
        };

        // Act
        var result = await _personalDataService.Decline(personalDataDatabaseModel);

        // Assert
        Assert.That(result, Is.False);
        _personalDataRepository.Verify(r => r.ChangeStatusAsync(personalDataId, PersonalDataStatus.Declined),
            Times.Never);
    }
}
