using Cdm.Data;
using Cdm.Data.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cdm.Tests.User;

public abstract class UserTestBase
{
    public static readonly string ValidUserName = "TestUser";
    public static readonly string ValidUserEmail = "test@example.com";
    public static readonly string ValidPassword = "hashed";
    public static readonly string InvalidUserName = "";
    public static readonly string InvalidUserEmail = "notanemail";
    public static readonly string InvalidPassword = "";

    protected Cdm.Data.Models.User CreateUserEntity(string name = null, string email = null, string password = null)
    {
        return new Cdm.Data.Models.User {
            UserName = name ?? ValidUserName,
            UserEmail = email ?? ValidUserEmail,
            Password = password ?? ValidPassword
        };
    }

    // Helper to mock DbSet<T> for IQueryable
    protected Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
    {
        var mockSet = new Mock<DbSet<T>>();
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        return mockSet;
    }

    // Helper to create DbContextOptions for AppDbContext
    protected DbContextOptions<AppDbContext> CreateDbContextOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>().Options;
    }
}
