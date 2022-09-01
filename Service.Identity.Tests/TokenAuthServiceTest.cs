using System.Security.Claims;
using Service.Identity.Models;
using Service.Identity.Services;
using Shared.Abstractions;

namespace Service.Identity.Tests;

public class TokenAuthServiceTest
{
    private TokenAuthService _tokenAuthService = null!;

    [SetUp]
    public void Setup()
    {
        const string secret =
            "XCAP05H6LoKvbRRa/QkqLNMI7cOHguaRyHzyg7n5qEkGjQmtBhz4SzYh4Fqwjyi3KJHlSXKPwVu2+bXr6CtpgQ==";
        const int expireInMinutes = 60;

        _tokenAuthService = new TokenAuthService(secret, expireInMinutes);
    }

    [Test]
    public void JwtTokenCanBeVerified()
    {
        // Arrange
        var user = new UserDatabaseModel(Guid.NewGuid(), "test@gmail.com", "password", UserStatus.Created);
        var token = _tokenAuthService.GenerateToken(user);

        // Act
        var claimsPrincipal = _tokenAuthService.VerifyToken(token);

        // Assert
        Assert.That(claimsPrincipal, Is.Not.Null);
    }

    [Test]
    public void JwtTokenContainsRequiredClaims()
    {
        // Arrange
        const string expectedId = "bb085d3d-0480-47e2-94f0-1c059eda9d92";
        const string expectedEmail = "test@gmail.com";
        const UserStatus expectedRole = UserStatus.Admin;
        var user = new UserDatabaseModel(Guid.Parse(expectedId), expectedEmail, "password", expectedRole);
        var token = _tokenAuthService.GenerateToken(user);

        // Act
        var claimsPrincipal = _tokenAuthService.VerifyToken(token);

        // Assert
        var id = claimsPrincipal?.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.PrimarySid);
        var email = claimsPrincipal?.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
        var role = claimsPrincipal?.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);

        Assert.Multiple(() =>
        {
            Assert.That(id?.Value, Is.EqualTo(expectedId));
            Assert.That(email?.Value, Is.EqualTo(expectedEmail));
            Assert.That(role?.Value, Is.EqualTo(expectedRole.AsString()));
        });
    }
}
