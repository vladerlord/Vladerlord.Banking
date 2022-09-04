using System.Text;
using Gateway.Root.PersonalData.Application;
using Gateway.Root.PersonalData.Domain;
using Microsoft.AspNetCore.Http;
using Moq;
using Shared.Grpc;
using Shared.Grpc.PersonalData;
using Shared.Grpc.PersonalData.Contracts;
using Shared.Grpc.PersonalData.Models;

namespace Gateway.Root.Tests.PersonalData;

public class PersonalDataServiceTest
{
    private Mock<IPersonalDataGrpcService> _personalDataGrpcService = null!;
    private PersonalDataService _personalDataService = null!;

    [SetUp]
    public void Setup()
    {
        _personalDataGrpcService = new Mock<IPersonalDataGrpcService>();
        _personalDataService = new PersonalDataService(_personalDataGrpcService.Object);
    }

    [Test]
    public async Task SendPersonalDataConfirmationRequestSuccessful()
    {
        // Arrange
        var personalDataId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        const string firstName = "first name";
        const string lastName = "last name";
        const string country = "country";
        const string city = "city";

        var ms = new MemoryStream(Encoding.ASCII.GetBytes("somefilecontent"));
        var files = new FormFileCollection
        {
            new FormFile(ms, 0, ms.Length, "filename", "filename.txt")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            }
        };
        var dto = new PersonalDataConfirmationDto(userId, firstName, lastName, country, city, files);
        var response = new ApplyPersonalDataGrpcResponse
        {
            GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Ok },
            PersonalData = new PersonalDataGrpcModel
            {
                Id = personalDataId,
                UserId = userId,
                FirstName = firstName,
                LastName = lastName,
                Country = country,
                City = city
            }
        };

        _personalDataGrpcService.Setup(s => s.ApplyPersonalDataAsync(It.Is<ApplyPersonalDataGrpcRequest>(r =>
                r.KycScans.Exists(scan =>
                    scan.FileName == "filename"
                    && scan.FileExtension == ".txt"
                    && scan.ContentType == "text/plain"
                ) &&
                r.PersonalData.FirstName == firstName &&
                r.PersonalData.LastName == lastName &&
                r.PersonalData.Country == country &&
                r.PersonalData.City == city
            )))
            .Returns(Task.FromResult(response));

        // Act
        var result = await _personalDataService.SendPersonalDataConfirmationRequest(dto);

        // Assert
        Assert.That(result.GrpcStatus.Status, Is.EqualTo(GrpcResponseStatus.Ok));
        Assert.That(result.Content?.Id, Is.EqualTo(personalDataId));
        Assert.That(result.Content?.UserId, Is.EqualTo(userId));
        Assert.That(result.Content?.FirstName, Is.EqualTo(firstName));
        Assert.That(result.Content?.LastName, Is.EqualTo(lastName));
        Assert.That(result.Content?.Country, Is.EqualTo(country));
        Assert.That(result.Content?.City, Is.EqualTo(city));
    }
}
