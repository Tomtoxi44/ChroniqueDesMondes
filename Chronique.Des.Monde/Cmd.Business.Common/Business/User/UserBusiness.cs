using Chronique.Des.Mondes.Data;
using Chronique.Des.Mondes.Data.Models;
using Cmd.Business.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Cmd.Business.Common.Business.User;

public class UserBusiness
{
    private readonly AppDbContext _dbContext;

    public UserBusiness(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsEmailTakenAsync(string email)
    {
        return await this._dbContext.Set<Users>().AnyAsync(u => u.UserEmail == email);
    }

    public async Task RegisterUserAsync(UserRequest userRequest)
    {
        if (await IsEmailTakenAsync(userRequest.UserEmail))
        {
            throw new InvalidOperationException("L'email est d�j� utilis�.");
        }

        var user = new Users()
        {
            Password = userRequest.Password,
            UserEmail = userRequest.UserEmail,
            UserName = userRequest.UserName,
        };

        this._dbContext.Set<Users>().Add(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Users?> GetUserByEmailAsync(string email)
    {
        return await this._dbContext.Set<Users>().FirstOrDefaultAsync(u => u.UserEmail == email);
    }
}