using Chronique.Des.Mondes.Data;
using Chronique.Des.Mondes.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Cdm.Business.Common.Business.Users;

using Cdm.Business.Common.Business.Users.Models;
using Models;

public class UserBusiness
{
    private readonly AppDbContext _dbContext;

    public UserBusiness(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsEmailTakenAsync(string email)
    {
        return await this._dbContext.Set<User>().AnyAsync(u => u.UserEmail == email);
    }

    public async Task RegisterUserAsync(UserRequest userRequest)
    {
        if (await IsEmailTakenAsync(userRequest.UserEmail))
            throw new InvalidOperationException("L'email est d�j� utilis�.");

        var user = new User()
        {
            Password = userRequest.Password,
            UserEmail = userRequest.UserEmail,
            UserName = userRequest.UserName,
        };

        this._dbContext.Set<User>().Add(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await this._dbContext.Set<User>().FirstOrDefaultAsync(u => u.UserEmail == email);
    }
}