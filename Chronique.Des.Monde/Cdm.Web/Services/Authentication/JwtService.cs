using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Cdm.Web.Services.Authentication;

public interface IJwtService
{
    ClaimsPrincipal? ValidateToken(string token);
    bool IsTokenValid(string token);
    string? GetUserIdFromToken(string token);
    string? GetEmailFromToken(string token);
}

public class JwtService : IJwtService
{
    private readonly ILogger<JwtService> _logger;

    public JwtService(ILogger<JwtService> logger)
    {
        _logger = logger;
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            
            var claims = jsonToken.Claims.ToList();
            var identity = new ClaimsIdentity(claims, "jwt");
            
            return new ClaimsPrincipal(identity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la validation du token JWT");
            return null;
        }
    }

    public bool IsTokenValid(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            
            // Vérifier si le token n'est pas expiré
            return jsonToken.ValidTo > DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la vérification de validité du token");
            return false;
        }
    }

    public string? GetUserIdFromToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            
            return jsonToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'extraction de l'ID utilisateur du token");
            return null;
        }
    }

    public string? GetEmailFromToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            
            return jsonToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'extraction de l'email du token");
            return null;
        }
    }
}