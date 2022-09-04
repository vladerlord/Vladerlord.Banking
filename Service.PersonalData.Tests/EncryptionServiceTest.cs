using System.Text;
using Service.PersonalData.Services;

namespace Service.PersonalData.Tests;

public class EncryptionServiceTest
{
    private EncryptionService _encryptionService = null!;

    [SetUp]
    public void Setup()
    {
        _encryptionService = new EncryptionService("MOn56M2W+dQ+RXxqD300GORRnRMOSK/nEJTpr2RzsmM=");
    }

    [Test]
    public void DecryptStringSuccessful()
    {
        // Arrange
        const string input = "some useful password";
        var iv = _encryptionService.GenerateIv();
        var encryptedInput = _encryptionService.Encrypt(input, iv);

        // Act
        var decrypted = _encryptionService.Decrypt(encryptedInput, iv);

        // Assert
        Assert.That(decrypted, Is.EqualTo(input));
    }

    [Test]
    public void DecryptStringWrongIv()
    {
        // Arrange
        const string input = "some useful password";
        var iv = _encryptionService.GenerateIv();
        var encryptedInput = _encryptionService.Encrypt(input, iv);

        // Act
        var decrypted = _encryptionService.Decrypt(encryptedInput, _encryptionService.GenerateIv());

        // Assert
        Assert.That(decrypted, Is.Not.EqualTo(input));
    }

    [Test]
    public void DecryptBytesSuccessful()
    {
        // Arrange
        var input = Encoding.ASCII.GetBytes("some useful password");
        var iv = Convert.FromBase64String(_encryptionService.GenerateIv());
        var encryptedInput = _encryptionService.Encrypt(input, iv);

        // Act
        var decrypted = _encryptionService.Decrypt(encryptedInput, iv);

        // Assert
        Assert.That(decrypted, Is.EqualTo(input));
    }

    [Test]
    public void DecryptBytesWrongIv()
    {
        // Arrange
        var input = Encoding.ASCII.GetBytes("some useful password");
        var iv = Convert.FromBase64String(_encryptionService.GenerateIv());
        var wrongIv = Convert.FromBase64String(_encryptionService.GenerateIv());
        var encryptedInput = _encryptionService.Encrypt(input, iv);

        // Act
        var decrypted = _encryptionService.Decrypt(encryptedInput, wrongIv);

        // Assert
        Assert.That(decrypted, Is.Not.EqualTo(input));
    }
}
