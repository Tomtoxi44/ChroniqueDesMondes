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

public class UserTu : UserTestBase
{
    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnUser_WhenExists()
    {
        var userList = new List<Cdm.Data.Models.User> { CreateUserEntity("Paul", "paul@example.com") };
        var mockSet = CreateMockDbSet(userList.AsQueryable());
        var mockContext = new Mock<Cdm.Data.AppDbContext>(CreateDbContextOptions());
        mockContext.Setup(c => c.Users).Returns(mockSet.Object);

        var userBusiness = new UserBusiness(mockContext.Object, null, null);
        var result = await userBusiness.GetUserByEmailAsync("paul@example.com");
        Assert.NotNull(result);
        Assert.Equal("Paul", result!.UserName);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnNull_WhenNotExists()
    {
        var userList = new List<Cdm.Data.Models.User>();
        var mockSet = CreateMockDbSet(userList.AsQueryable());
        var mockContext = new Mock<Cdm.Data.AppDbContext>(CreateDbContextOptions());
        mockContext.Setup(c => c.Users).Returns(mockSet.Object);

        var userBusiness = new UserBusiness(mockContext.Object, null, null);
        var result = await userBusiness.GetUserByEmailAsync("notfound@example.com");
        Assert.Null(result);
    }
}
