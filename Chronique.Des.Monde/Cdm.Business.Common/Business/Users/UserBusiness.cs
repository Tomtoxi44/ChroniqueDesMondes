using Cdm.Data;
using Cdm.Data.Models;
using Cdm.Common;
using Microsoft.EntityFrameworkCore;

namespace Cdm.Business.Common.Business.Users;

public class UserBusiness
{
    private readonly PasswordService _passwordService;
    private readonly JwtService _jwtService;
    private readonly AppDbContext _dbContext;

    public UserBusiness(AppDbContext dbContext, PasswordService passwordService, JwtService jwtService)
    {
        _dbContext = dbContext;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    public async Task<User> CreateUserAsync(string userName, string userEmail, string password)
    {
        // Check if user already exists
        var existingUser = await GetUserByEmailAsync(userEmail);
        if (existingUser != null)
        {
            throw new ArgumentException("User with this email already exists.");
        }

        // Hash the password
        var hashedPassword = _passwordService.HashPassword(password);

        // Create new user
        var user = new User
        {
            UserName = userName,
            UserEmail = userEmail,
            Password = hashedPassword
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.UserEmail == email);
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<string?> AuthenticateAsync(string email, string password)
    {
        var user = await GetUserByEmailAsync(email);
        if (user == null)
        {
            return null;
        }

        // Verify password
        if (!_passwordService.VerifyPassword(password, user.Password))
        {
            return null;
        }

        // Generate JWT token
        return _jwtService.GenerateToken(user.Id, user.UserName, user.UserEmail);
    }

    public bool ValidateToken(string token)
    {
        return _jwtService.ValidateToken(token);
    }

    public (int userId, string userName, string userEmail)? GetUserInfoFromToken(string token)
    {
        return _jwtService.GetUserInfoFromToken(token);
    }
}