using Microsoft.AspNetCore.Identity;

namespace Cdm.DataSeeder.Services;

/// <summary>
/// Service pour le hachage et la vérification des mots de passe
/// </summary>
public class PasswordService
{
    private readonly PasswordHasher<string> _passwordHasher = new();

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword("", password);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword("", hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success;
    }
}