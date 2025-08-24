using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Cdm.Common;

public class JwtService
{
    private readonly string _secretKey;
    private readonly string _issuer;

    public JwtService()
    {
        // Configuration temporaire (à externaliser)
        _secretKey = "your-super-secret-key-that-is-at-least-32-characters-long";
        _issuer = "ChroniqueDesMondes";
    }

    public string GenerateToken(int userId, string userName, string userEmail)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email, userEmail)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = _issuer,
            Audience = _issuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public bool ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _issuer,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public (int userId, string userName, string userEmail)? GetUserInfoFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _issuer,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = principal.FindFirst(ClaimTypes.Name)?.Value;
            var userEmail = principal.FindFirst(ClaimTypes.Email)?.Value;

            if (int.TryParse(userIdClaim, out var userId) && !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userEmail))
            {
                return (userId, userName, userEmail);
            }
        }
        catch
        {
            // Token invalide
        }
        return null;
    }
}