using System.Security.Claims;

namespace Cdm.Web.Services.Authentication;

public interface IAuthenticationService
{
    Task<bool> LoginAsync(string username, string password);
    Task<bool> RegisterAsync(string username, string email, string password);
    Task LogoutAsync();
    Task<ClaimsPrincipal?> GetCurrentUserAsync();
    string? GetCurrentUserToken();
    bool IsAuthenticated { get; }
}