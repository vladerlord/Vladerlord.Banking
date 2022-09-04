using System.Text;
using Moq;
using Service.PersonalData.Abstractions;
using Service.PersonalData.Models;
using Service.PersonalData.Services;
using Shared.Grpc.PersonalData.Models;

namespace Service.PersonalData.Tests;

public class KycScanEncryptionServiceTest
{
    private Mock<IPersonalDataRepository> _personalDataRepository = null!;
    private KycScanEncryptionService _kycEncryptionService = null!;

    [SetUp]
    public void Setup()
    {
        _personalDataRepository = new Mock<IPersonalDataRepository>();

        _kycEncryptionService = new KycScanEncryptionService(
            new EncryptionService("MOn56M2W+dQ+RXxqD300GORRnRMOSK/nEJTpr2RzsmM="),
            _personalDataRepository.Object
        );
    }

    [Test]
    public void DecryptGrpcModelSuccessful()
    {
        // Arrange
        const string iv = "OR/dPGUeW5gwPoaMvQ0A9Q==";
        var content = Encoding.ASCII.GetBytes("somepngimagecontent");

        var createRequest = new KycScanCreateGrpcModel
        {
            FileName = "test",
            FileExtension = ".png",
            ContentType = "image/png",
            Content = content
        };
        _kycEncryptionService.EncryptCreateGrpcModel(createRequest, iv);
        var databaseModel = createRequest.ToDatabaseModel(Guid.NewGuid());
        var grpcModel = databaseModel.ToGrpcModel(createRequest.Content);

        // Act
        _kycEncryptionService.DecryptGrpcModel(grpcModel, iv);

        // Assert
        Assert.That(grpcModel.Content, Is.EqualTo(content));
    }

    [Test]
    public void DecryptGrpcModelWrongIv()
    {
        // Arrange
        const string iv = "OR/dPGUeW5gwPoaMvQ0A9Q==";
        const string wrongIv = "OR/dPGUeW5gwPoaMvQ0A9E==";
        var content = Encoding.ASCII.GetBytes("somepngimagecontent");

        var createRequest = new KycScanCreateGrpcModel
        {
            FileName = "test",
            FileExtension = ".png",
            ContentType = "image/png",
            Content = content
        };
        _kycEncryptionService.EncryptCreateGrpcModel(createRequest, iv);
        var databaseModel = createRequest.ToDatabaseModel(Guid.NewGuid());
        var grpcModel = databaseModel.ToGrpcModel(createRequest.Content);

        // Act
        _kycEncryptionService.DecryptGrpcModel(grpcModel, wrongIv);

        // Assert
        Assert.That(grpcModel.Content, Is.Not.EqualTo(content));
    }
}
