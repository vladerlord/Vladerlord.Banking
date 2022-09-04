using System.Text;
using Microsoft.Extensions.Logging;
using Moq;
using Service.PersonalData.Abstractions;
using Shared.Grpc;
using Shared.Grpc.PersonalData.Contracts;
using Shared.Grpc.PersonalData.Models;

namespace Service.PersonalData.Tests;

public class KycScanGrpcServiceTest
{
    private Mock<IKycScansService> _kycScansService = null!;
    private Mock<ILogger<KycScanGrpcService>> _logger = null!;
    private KycScanGrpcService _kycScanGrpcService = null!;

    [SetUp]
    public void Setup()
    {
        _kycScansService = new Mock<IKycScansService>();
        _logger = new Mock<ILogger<KycScanGrpcService>>();
        _kycScanGrpcService = new KycScanGrpcService(_kycScansService.Object, _logger.Object);
    }

    [Test]
    public async Task FindByIdSuccessful()
    {
        // Arrange
        var kycScanId = Guid.NewGuid();
        var personalDataId = Guid.NewGuid();
        var request = new FindKycScanByIdGrpcRequest
        {
            Id = kycScanId
        };
        const string fileExtension = ".png";
        const string originalName = "test";
        const string contentType = "image/png";
        var content = Encoding.ASCII.GetBytes("some string");
        var kycScanGrpcModel = new KycScanGrpcModel
        {
            FileName = kycScanId,
            PersonalDataId = personalDataId,
            FileExtension = ".png",
            OriginalName = "test",
            ContentType = "image/png",
            Content = content
        };

        _kycScansService.Setup(s => s.FindById(kycScanId))
            .Returns(Task.FromResult<KycScanGrpcModel?>(kycScanGrpcModel));

        // Act
        var result = await _kycScanGrpcService.FindByIdAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.GrpcResponse.Status, Is.EqualTo(GrpcResponseStatus.Ok));
            Assert.That(result.KycScan?.FileName, Is.EqualTo(kycScanId));
            Assert.That(result.KycScan?.PersonalDataId, Is.EqualTo(personalDataId));
            Assert.That(result.KycScan?.FileExtension, Is.EqualTo(fileExtension));
            Assert.That(result.KycScan?.OriginalName, Is.EqualTo(originalName));
            Assert.That(result.KycScan?.ContentType, Is.EqualTo(contentType));
            Assert.That(result.KycScan?.Content, Is.EqualTo(content));
        });
    }
}
