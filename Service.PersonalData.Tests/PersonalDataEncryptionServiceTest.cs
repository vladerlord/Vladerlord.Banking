using Service.PersonalData.Models;
using Service.PersonalData.Services;

namespace Service.PersonalData.Tests;

public class PersonalDataEncryptionServiceTest
{
    private EncryptionService _encryptionService = null!;
    private PersonalDataEncryptionService _personalDataEncryptionService = null!;

    [SetUp]
    public void Setup()
    {
        _encryptionService = new EncryptionService("MOn56M2W+dQ+RXxqD300GORRnRMOSK/nEJTpr2RzsmM=");
        _personalDataEncryptionService = new PersonalDataEncryptionService(_encryptionService);
    }

    [Test]
    public void EncryptSuccessful()
    {
        // Arrange
        var personalDataId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        const string iv = "OR/dPGUeW5gwPoaMvQ0A9Q==";
        const string firstName = "some name";
        const string lastName = "some last name";
        const string country = "Ukraine";
        const string city = "Kiev";

        var personalDataModel = new PersonalDataDatabaseModel
        {
            Id = personalDataId,
            UserId = userId,
            FirstName = firstName,
            LastName = lastName,
            Country = country,
            City = city,
            Iv = iv,
            Status = PersonalDataStatus.Approved
        };

        // Act
        _personalDataEncryptionService.Encrypt(personalDataModel);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(personalDataModel.Id, Is.EqualTo(personalDataId));
            Assert.That(personalDataModel.UserId, Is.EqualTo(userId));
            Assert.That(personalDataModel.FirstName, Is.Not.EqualTo(firstName));
            Assert.That(personalDataModel.LastName, Is.Not.EqualTo(lastName));
            Assert.That(personalDataModel.Country, Is.EqualTo(country));
            Assert.That(personalDataModel.City, Is.EqualTo(city));
            Assert.That(personalDataModel.Iv, Is.EqualTo(iv));
            Assert.That(personalDataModel.Status, Is.EqualTo(PersonalDataStatus.Approved));
        });
    }
    
    [Test]
    public void DecryptSuccessful()
    {
        // Arrange
        var personalDataId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        const string iv = "OR/dPGUeW5gwPoaMvQ0A9Q==";
        const string firstName = "some name";
        const string lastName = "some last name";
        const string country = "Ukraine";
        const string city = "Kiev";

        var personalDataModel = new PersonalDataDatabaseModel
        {
            Id = personalDataId,
            UserId = userId,
            FirstName = firstName,
            LastName = lastName,
            Country = country,
            City = city,
            Iv = iv,
            Status = PersonalDataStatus.Approved
        };
        _personalDataEncryptionService.Encrypt(personalDataModel);

        // Act
        _personalDataEncryptionService.Decrypt(personalDataModel);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(personalDataModel.Id, Is.EqualTo(personalDataId));
            Assert.That(personalDataModel.UserId, Is.EqualTo(userId));
            Assert.That(personalDataModel.FirstName, Is.EqualTo(firstName));
            Assert.That(personalDataModel.LastName, Is.EqualTo(lastName));
            Assert.That(personalDataModel.Country, Is.EqualTo(country));
            Assert.That(personalDataModel.City, Is.EqualTo(city));
            Assert.That(personalDataModel.Iv, Is.EqualTo(iv));
            Assert.That(personalDataModel.Status, Is.EqualTo(PersonalDataStatus.Approved));
        });
    }
}
