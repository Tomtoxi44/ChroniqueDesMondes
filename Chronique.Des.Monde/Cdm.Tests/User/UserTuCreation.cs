using Cdm.Business.Common.Business.Users;
using Cdm.Data;
using Cdm.Data.Models;
using Moq;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Cdm.Tests.User;

public class UserTuCreation : UserTestBase
{
    [Fact]
    public async Task CreateUserAsync_ShouldCreateUser()
    {
        var userList = new List<Cdm.Data.Models.User>();
        var mockSet = CreateMockDbSet(userList.AsQueryable());
        mockSet.Setup(m => m.Add(It.IsAny<Cdm.Data.Models.User>())).Callback<Cdm.Data.Models.User>(u => userList.Add(u));
        var mockContext = new Mock<Cdm.Data.AppDbContext>(CreateDbContextOptions());
        mockContext.Setup(c => c.Users).Returns(mockSet.Object);
        mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

        var userBusiness = new UserBusiness(mockContext.Object, null, null);
        var userName = "Marie";
        var userEmail = "marie@example.com";
        var password = "password123";

        var user = await userBusiness.CreateUserAsync(userName, userEmail, password);

        Assert.NotNull(user);
        Assert.Equal(userName, user.UserName);
        Assert.Equal(userEmail, user.UserEmail);
        Assert.Single(userList);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldThrow_WhenEmailExists()
    {
        var userList = new List<Cdm.Data.Models.User> { CreateUserEntity("Marie", "marie@example.com") };
        var mockSet = CreateMockDbSet(userList.AsQueryable());
        var mockContext = new Mock<Cdm.Data.AppDbContext>(CreateDbContextOptions());
        mockContext.Setup(c => c.Users).Returns(mockSet.Object);

        var userBusiness = new UserBusiness(mockContext.Object, null, null);
        var userName = "Marie";
        var userEmail = "marie@example.com";
        var password = "password123";

        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await userBusiness.CreateUserAsync(userName, userEmail, password);
        });
    }
}
