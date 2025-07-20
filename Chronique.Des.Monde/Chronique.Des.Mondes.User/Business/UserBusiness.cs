using Chronique.Des.Mondes.Data;
using Chronique.Des.Mondes.Data.Models;
using Chronique.Des.Mondes.User.Model;
using Microsoft.EntityFrameworkCore;

public class UserBusiness
{
    private readonly AppDbContext _dbContext;

    public UserBusiness(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsEmailTakenAsync(string email)
    {
        return await _dbContext.Users.AnyAsync(u => u.UserEmail == email);
    }

    public async Task RegisterUserAsync(UserRequest userRequest)
    {
        if (await IsEmailTakenAsync(userRequest.UserEmail))
        {
            throw new InvalidOperationException("L'email est déjà utilisé.");
        }

        var user = new Users()
        {
            Password = userRequest.Password,
            UserEmail = userRequest.UserEmail,
            UserName = userRequest.UserName,
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Users?> GetUserByEmailAsync(string email)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.UserEmail == email);
    }
}