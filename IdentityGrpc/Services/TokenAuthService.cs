using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace IdentityGrpc.Services;

public class TokenAuthService
{
    private readonly string _secret;
    private readonly int _expirationMinutes;

    public TokenAuthService(string secret, int expirationMinutes)
    {
        _secret = secret;
        _expirationMinutes = expirationMinutes;
    }

    public string GenerateToken(string email)
    {
        var key = Convert.FromBase64String(_secret);
        var securityKey = new SymmetricSecurityKey(key);
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, email) }),
            Expires = DateTime.UtcNow.AddMinutes(_expirationMinutes),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateJwtSecurityToken(descriptor);

        return handler.WriteToken(token);
    }

    public bool VerifyToken(string token)
    {
        if (token.Length == 0)
            return false;

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(_secret))
        };
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        try
        {
            jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out _);

            return true;
        }
        catch
        {
            return false;
        }
    }
}