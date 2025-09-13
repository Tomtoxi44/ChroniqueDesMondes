using Microsoft.AspNetCore.Mvc;
using Cdm.Common;
using System.ComponentModel.DataAnnotations;

namespace Cdm.ApiService.Endpoints;

/// <summary>
/// Endpoints d'authentification pour l'API
/// </summary>
public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var authGroup = app.MapGroup("/api/auth")
            .WithTags("Authentication")
            .WithOpenApi();

        // POST /api/auth/login - Connexion utilisateur
        authGroup.MapPost("/login", async (
            [FromBody] LoginRequest request,
            ILogger<Program> logger) =>
        {
            try
            {
                logger.LogInformation("🔐 Tentative de connexion pour {Email}", request.Email);

                // TODO: Remplacer par une vraie validation avec base de données
                // Pour l'instant, on accepte n'importe quel email/mot de passe pour les tests
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                {
                    logger.LogWarning("❌ Champs email ou mot de passe vides");
                    return Results.BadRequest(new AuthResponse 
                    { 
                        Success = false, 
                        Error = "Email et mot de passe requis" 
                    });
                }

                // Simulation d'un utilisateur valide (à remplacer par DB lookup)
                var jwtService = new JwtService();
                var token = jwtService.GenerateToken(
                    userId: 1,
                    userName: request.Email.Split('@')[0], // Utilise la partie avant @ comme nom
                    userEmail: request.Email
                );

                var response = new AuthResponse
                {
                    Success = true,
                    Token = token,
                    User = new UserInfo
                    {
                        Id = 1,
                        UserName = request.Email.Split('@')[0],
                        UserEmail = request.Email
                    }
                };

                logger.LogInformation("✅ Connexion réussie pour {Email}", request.Email);
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "💥 Erreur lors de la connexion pour {Email}", request.Email);
                return Results.Problem(
                    title: "Erreur de connexion",
                    detail: "Une erreur inattendue s'est produite",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .WithName("Login")
        .WithSummary("Connexion utilisateur avec JWT")
        .Produces<AuthResponse>(StatusCodes.Status200OK)
        .Produces<AuthResponse>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // POST /api/auth/register - Inscription utilisateur
        authGroup.MapPost("/register", async (
            [FromBody] RegisterRequest request,
            ILogger<Program> logger) =>
        {
            try
            {
                logger.LogInformation("📝 Tentative d'inscription pour {Email}", request.Email);

                // Validation des champs
                if (string.IsNullOrEmpty(request.UserName) || 
                    string.IsNullOrEmpty(request.Email) || 
                    string.IsNullOrEmpty(request.Password))
                {
                    logger.LogWarning("❌ Champs requis manquants pour l'inscription");
                    return Results.BadRequest(new AuthResponse 
                    { 
                        Success = false, 
                        Error = "Nom d'utilisateur, email et mot de passe requis" 
                    });
                }

                // Validation de l'email
                if (!IsValidEmail(request.Email))
                {
                    logger.LogWarning("❌ Format d'email invalide: {Email}", request.Email);
                    return Results.BadRequest(new AuthResponse 
                    { 
                        Success = false, 
                        Error = "Format d'email invalide" 
                    });
                }

                // TODO: Vérifier que l'email n'existe pas déjà en base
                // TODO: Hasher le mot de passe avant stockage
                // TODO: Sauvegarder en base de données

                // Pour l'instant, on crée directement le token
                var jwtService = new JwtService();
                var token = jwtService.GenerateToken(
                    userId: Random.Shared.Next(1000, 9999), // ID temporaire
                    userName: request.UserName,
                    userEmail: request.Email
                );

                var response = new AuthResponse
                {
                    Success = true,
                    Token = token,
                    User = new UserInfo
                    {
                        Id = Random.Shared.Next(1000, 9999),
                        UserName = request.UserName,
                        UserEmail = request.Email
                    }
                };

                logger.LogInformation("✅ Inscription réussie pour {Email}", request.Email);
                return Results.Created($"/api/users/{response.User.Id}", response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "💥 Erreur lors de l'inscription pour {Email}", request.Email);
                return Results.Problem(
                    title: "Erreur d'inscription",
                    detail: "Une erreur inattendue s'est produite",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .WithName("Register")
        .WithSummary("Inscription d'un nouvel utilisateur")
        .Produces<AuthResponse>(StatusCodes.Status201Created)
        .Produces<AuthResponse>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET /api/auth/validate - Validation d'un token JWT
        authGroup.MapGet("/validate", async (
            HttpContext context,
            ILogger<Program> logger) =>
        {
            try
            {
                var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
                
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    logger.LogWarning("🔒 Token d'autorisation manquant ou invalide");
                    return Results.Unauthorized();
                }

                var token = authHeader.Substring("Bearer ".Length);
                var jwtService = new JwtService();

                if (jwtService.ValidateToken(token))
                {
                    var userInfo = jwtService.GetUserInfoFromToken(token);
                    if (userInfo.HasValue)
                    {
                        logger.LogInformation("✅ Token valide pour utilisateur {UserId}", userInfo.Value.UserId);
                        return Results.Ok(new { Valid = true, User = userInfo.Value });
                    }
                }

                logger.LogWarning("❌ Token invalide ou expiré");
                return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "💥 Erreur lors de la validation du token");
                return Results.Problem(
                    title: "Erreur de validation",
                    detail: "Une erreur inattendue s'est produite",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .WithName("ValidateToken")
        .WithSummary("Validation d'un token JWT")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET /api/auth/me - Informations utilisateur actuel
        authGroup.MapGet("/me", async (
            HttpContext context,
            ILogger<Program> logger) =>
        {
            try
            {
                var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
                
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Results.Unauthorized();
                }

                var token = authHeader.Substring("Bearer ".Length);
                var jwtService = new JwtService();
                var userInfo = jwtService.GetUserInfoFromToken(token);

                if (userInfo.HasValue)
                {
                    var user = new UserInfo
                    {
                        Id = userInfo.Value.UserId,
                        UserName = userInfo.Value.UserName,
                        UserEmail = userInfo.Value.UserEmail
                    };

                    logger.LogInformation("👤 Informations utilisateur récupérées pour {UserId}", userInfo.Value.UserId);
                    return Results.Ok(user);
                }

                return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "💥 Erreur lors de la récupération des informations utilisateur");
                return Results.Problem(
                    title: "Erreur utilisateur",
                    detail: "Une erreur inattendue s'est produite",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .RequireAuthorization() // Cet endpoint nécessite l'authentification
        .WithName("GetCurrentUser")
        .WithSummary("Récupère les informations de l'utilisateur connecté")
        .Produces<UserInfo>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    /// <summary>
    /// Validation simple d'un email
    /// </summary>
    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

#region DTOs

/// <summary>
/// Requête de connexion
/// </summary>
public record LoginRequest
{
    [Required(ErrorMessage = "L'email est requis")]
    [EmailAddress(ErrorMessage = "Format d'email invalide")]
    public string Email { get; init; } = "";

    [Required(ErrorMessage = "Le mot de passe est requis")]
    [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères")]
    public string Password { get; init; } = "";
}

/// <summary>
/// Requête d'inscription
/// </summary>
public record RegisterRequest
{
    [Required(ErrorMessage = "Le nom d'utilisateur est requis")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Le nom d'utilisateur doit contenir entre 3 et 50 caractères")]
    public string UserName { get; init; } = "";

    [Required(ErrorMessage = "L'email est requis")]
    [EmailAddress(ErrorMessage = "Format d'email invalide")]
    public string Email { get; init; } = "";

    [Required(ErrorMessage = "Le mot de passe est requis")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères")]
    public string Password { get; init; } = "";
}

/// <summary>
/// Réponse d'authentification
/// </summary>
public record AuthResponse
{
    public bool Success { get; init; }
    public string? Token { get; init; }
    public UserInfo? User { get; init; }
    public string? Error { get; init; }
}

/// <summary>
/// Informations utilisateur
/// </summary>
public record UserInfo
{
    public int Id { get; init; }
    public string UserName { get; init; } = "";
    public string UserEmail { get; init; } = "";
}

#endregion