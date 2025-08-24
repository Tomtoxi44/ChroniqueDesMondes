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
        this._dbContext = dbContext;
        this._passwordService = passwordService;
        this._jwtService = jwtService;
    }

    public async Task<User> CreateUserAsync(string userName, string userEmail, string password)
    {
        // Check if user already exists
        var existingUser = await this.GetUserByEmailAsync(userEmail);
        if (existingUser != null)
        {
            throw new ArgumentException("User with this email already exists.");
        }

        // Hash the password
        var hashedPassword = this._passwordService.HashPassword(password);

        // Create new user
        var user = new User
        {
            UserName = userName,
            UserEmail = userEmail,
            Password = hashedPassword
        };

        this._dbContext.Users.Add(user);
        await this._dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await this._dbContext.Users
            .FirstOrDefaultAsync(u => u.UserEmail == email);
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await this._dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<string?> AuthenticateAsync(string email, string password)
    {
        var user = await this.GetUserByEmailAsync(email);
        if (user == null)
        {
            return null;
        }

        // Verify password
        if (!this._passwordService.VerifyPassword(password, user.Password))
        {
            return null;
        }

        // Generate JWT token
        return this._jwtService.GenerateToken(user.Id, user.UserName, user.UserEmail);
    }

    public bool ValidateToken(string token)
    {
        return this._jwtService.ValidateToken(token);
    }

    public (int UserId, string UserName, string UserEmail)? GetUserInfoFromToken(string token)
    {
        return this._jwtService.GetUserInfoFromToken(token);
    }
}