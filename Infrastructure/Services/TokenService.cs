using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.IServices;
using Application.Settings;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly SymmetricSecurityKey _secretKey;
    public TokenService(IOptions<JwtSettings> options)
    {
        _jwtSettings = options.Value;
        _secretKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Key)
        );
    }

    public string GenerateAccessToken(User user)
    {
        var issuer = _jwtSettings.Issuer;
        var audience = _jwtSettings.Audience;
        var expireHours = _jwtSettings.ExpireHours;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(expireHours),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                _secretKey,
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rn = RandomNumberGenerator.Create();
        rn.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
