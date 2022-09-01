using Service.Identity.Services;

namespace Service.Identity.Tests;

public class HashServiceTest
{
    private HashService _hashService = null!;

    [SetUp]
    public void Setup()
    {
        _hashService = new HashService();
    }

    [Test]
    public void HashCanBeVerified()
    {
        // Arrange
        var input = "QwedaSD6tq76qwr;^&asdmSND-wewe";
        var hashed = _hashService.Hash(input);

        // Act
        var isEqual = _hashService.VerifyHash(input, hashed);

        // Assert
        Assert.That(isEqual, Is.True);
    }
}
