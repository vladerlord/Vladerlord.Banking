using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Service.Identity.Models;
using Shared.Abstractions;

namespace Service.Identity.Services;

public class TokenAuthService
{
    private readonly string _secret;
    private readonly int _expirationMinutes;

    public TokenAuthService(string secret, int expirationMinutes)
    {
        _secret = secret;
        _expirationMinutes = expirationMinutes;
    }

    public string GenerateToken(UserDatabaseModel user)
    {
        var key = Convert.FromBase64String(_secret);
        var securityKey = new SymmetricSecurityKey(key);
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.PrimarySid, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Status.AsString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(_expirationMinutes),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateJwtSecurityToken(descriptor);

        return handler.WriteToken(token);
    }

    public ClaimsPrincipal? VerifyToken(string token)
    {
        if (token.Length == 0)
            return null;

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(_secret))
        };
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        try
        {
            return jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out _);
        }
        catch
        {
            return null;
        }
    }
}
