using System.Text;
using Moq;
using Service.PersonalData.Abstractions;
using Service.PersonalData.Models;
using Service.PersonalData.Services;
using Shared.Grpc.PersonalData.Models;

namespace Service.PersonalData.Tests;

public class KycScansServiceTest
{
    private Mock<IKycScansFileExplorer> _localKycScansFileService = null!;
    private Mock<IKycScanRepository> _kycScanRepository = null!;
    private Mock<IPersonalDataRepository> _personalDataRepository = null!;
    private EncryptionService _encryptionService = null!;
    private KycScanEncryptionService _kycScanEncryptionService = null!;
    private KycScansService _kycScansService = null!;

    [SetUp]
    public void Setup()
    {
        _localKycScansFileService = new Mock<IKycScansFileExplorer>();
        _kycScanRepository = new Mock<IKycScanRepository>();
        _personalDataRepository = new Mock<IPersonalDataRepository>();
        _encryptionService = new EncryptionService("MOn56M2W+dQ+RXxqD300GORRnRMOSK/nEJTpr2RzsmM=");
        _kycScanEncryptionService = new KycScanEncryptionService(
            _encryptionService,
            _personalDataRepository.Object
        );

        _kycScansService = new KycScansService(
            _localKycScansFileService.Object,
            _kycScanRepository.Object,
            _kycScanEncryptionService
        );
    }

    [Test]
    public async Task CreatedSuccessfully()
    {
        // Arrange
        var personalDataId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        const string iv = "OR/dPGUeW5gwPoaMvQ0A9Q==";
        var content = Encoding.ASCII.GetBytes("somepngimagecontent");

        var personalDataModel = new PersonalDataDatabaseModel
        {
            Id = personalDataId,
            UserId = userId,
            FirstName = "some name",
            LastName = "some last name",
            Country = "Ukraine",
            City = "Kiev",
            Iv = iv,
            Status = PersonalDataStatus.Approved
        };
        var createRequest = new KycScanCreateGrpcModel
        {
            FileName = "test",
            FileExtension = ".png",
            ContentType = "image/png",
            Content = content
        };
        var kycScans = new List<KycScanCreateGrpcModel> { createRequest };

        // Act
        await _kycScansService.Create(kycScans, personalDataModel);

        // Assert
        _localKycScansFileService.Verify(s => s.SaveKycScan(createRequest, It.IsAny<string>()), Times.Once);
        _kycScanRepository.Verify(r => r.CreateAsync(It.Is<KycScanDatabaseModel>(m =>
            m.PersonalDataId == personalDataId &&
            m.FileExtension == ".png" &&
            m.OriginalName == "test" &&
            m.ContentType == "image/png"
        )));
    }

    [Test]
    public async Task DeleteAllByPersonalDataIdSuccessful()
    {
        // Arrange
        var personalDataId = Guid.NewGuid();
        var kycScanDatabaseModel = new KycScanDatabaseModel
        {
            FileName = Guid.NewGuid(),
            FileExtension = ".png",
            ContentType = "image/png",
            PersonalDataId = personalDataId,
            OriginalName = "test"
        };

        _kycScanRepository.Setup(r => r.GetByPersonalDataIdAsync(personalDataId))
            .Returns(Task.FromResult<IEnumerable<KycScanDatabaseModel>>(new[] { kycScanDatabaseModel }));

        // Act
        await _kycScansService.DeleteAllByPersonalDataId(personalDataId);

        // Assert
        _localKycScansFileService.Verify(s => s.DeleteKycScan(kycScanDatabaseModel), Times.Once);
        _kycScanRepository.Verify(r => r.DeleteByPersonalDataIdAsync(personalDataId), Times.Once);
    }

    [Test]
    public async Task FindByIdDecryptedSuccessfully()
    {
        // Arrange
        var kycScanId = Guid.NewGuid();
        var personalDataId = Guid.NewGuid();
        const string iv = "OR/dPGUeW5gwPoaMvQ0A9Q==";
        var kycScanDatabaseModel = new KycScanDatabaseModel
        {
            FileName = kycScanId,
            FileExtension = ".png",
            ContentType = "image/png",
            PersonalDataId = personalDataId,
            OriginalName = "test"
        };
        var personalDataDatabaseModel = new PersonalDataDatabaseModel { Iv = iv };
        var decodedContent = Encoding.ASCII.GetBytes("some decrypted string");
        var encryptedContent = _encryptionService.Encrypt(decodedContent, Convert.FromBase64String(iv));

        _kycScanRepository.Setup(r => r.FindByIdAsync(kycScanId))
            .Returns(Task.FromResult<KycScanDatabaseModel?>(kycScanDatabaseModel));
        _localKycScansFileService.Setup(s => s.GetKyсScanContentAsync(kycScanDatabaseModel))
            .Returns(Task.FromResult(encryptedContent));
        _personalDataRepository.Setup(r => r.FindByIdAsync(personalDataId))
            .Returns(Task.FromResult<PersonalDataDatabaseModel?>(personalDataDatabaseModel));

        // Act
        var result = await _kycScansService.FindById(kycScanId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result?.Content, Is.EqualTo(decodedContent));
            Assert.That(result?.FileName, Is.EqualTo(kycScanId));
            Assert.That(result?.FileExtension, Is.EqualTo(".png"));
            Assert.That(result?.ContentType, Is.EqualTo("image/png"));
            Assert.That(result?.OriginalName, Is.EqualTo("test"));
        });
    }
}
