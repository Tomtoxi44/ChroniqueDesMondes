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

public class UserTuGet : UserTestBase
{
    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser_WhenExists()
    {
        var user = CreateUserEntity();
        user.Id = 42;
        var userList = new List<Cdm.Data.Models.User> { user };
        var mockSet = CreateMockDbSet(userList.AsQueryable());
        mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync(user);
        var mockContext = new Mock<Cdm.Data.AppDbContext>(CreateDbContextOptions());
        mockContext.Setup(c => c.Users).Returns(mockSet.Object);

        var userBusiness = new UserBusiness(mockContext.Object, null, null);
        var result = await userBusiness.GetUserByIdAsync(42);
        Assert.NotNull(result);
        Assert.Equal(ValidUserEmail, result!.UserEmail);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        var userList = new List<Cdm.Data.Models.User>();
        var mockSet = CreateMockDbSet(userList.AsQueryable());
        mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync((Cdm.Data.Models.User)null!);
        var mockContext = new Mock<Cdm.Data.AppDbContext>(CreateDbContextOptions());
        mockContext.Setup(c => c.Users).Returns(mockSet.Object);

        var userBusiness = new UserBusiness(mockContext.Object, null, null);
        var result = await userBusiness.GetUserByIdAsync(9999);
        Assert.Null(result);
    }
}
