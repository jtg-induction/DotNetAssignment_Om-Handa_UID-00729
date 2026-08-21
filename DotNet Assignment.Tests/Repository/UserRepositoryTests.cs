using DotNet_Assignment.Data;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Models.Enums;
using DotNet_Assignment.Repository.Address;
using DotNet_Assignment.Repository.RefreshTokens;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Tests.Utils.RepositoryHelpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Assignment.Tests.Repository
{
    [TestFixture]
    public class UserRepositoryTests
    {
        /// <summary>
        /// GetUserByEmailAsync Function  Found user - Returns User
        /// </summary>
        /// <returns>User</returns>
        [Test]
        public async Task GetUserByEmailAsync_ReturnsUser()
        {
            var requestUserEmail= "user@email.com";

            var data = new List<User>
                {
                    new User
                    {
                       UserId= Guid.NewGuid(),
                       Email=requestUserEmail,
                       Password= "Password",
                       Name="Name",
                       PhoneNumber= "9191919191",
                       Role=UserRoles.User,
                       Balance=1000m,
                       CreatedAt= DateTime.UtcNow,
                       UpdatedAt= DateTime.UtcNow,
                       IsDeleted= false
                    }
                }.AsQueryable();

            var mockContext = BuildMockContext(data);

            var repository = new UserRepository(mockContext.Object);
            var result = await repository.GetUserByEmailAsync(requestUserEmail);

            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// GetUserByEmailAsync Function - Did not Find user - Returns null
        /// </summary>
        /// <returns>null</returns>
        [Test]
        public async Task GetUserByEmailAsync_ReturnsNull()
        {
            var requestUserEmail = "user@email.com";

            var data = new List<User>
                {
                    new User
                    {
                       UserId= Guid.NewGuid(),
                       Email="WrongEmail@gmail.com",
                       Password= "Password",
                       Name="Name",
                       PhoneNumber= "9191919191",
                       Role=UserRoles.User,
                       Balance=1000m,
                       CreatedAt= DateTime.UtcNow,
                       UpdatedAt= DateTime.UtcNow,
                       IsDeleted= false
                    }
                }.AsQueryable();

            var mockContext = BuildMockContext(data);

            var repository = new UserRepository(mockContext.Object);
            var result = await repository.GetUserByEmailAsync(requestUserEmail);

            Assert.That(result, Is.Null);
        }

        /// <summary>
        /// FindUserByEmailAsync Function - Found user - Returns True
        /// </summary>
        /// <returns>true</returns>
        [Test]
        public async Task FindUserByEmailAsync_ReturnsTrue()
        {
            var requestUserEmail = "user@email.com";

            var data = new List<User>
                {
                    new User
                    {
                       UserId= Guid.NewGuid(),
                       Email=requestUserEmail,
                       Password= "Password",
                       Name="Name",
                       PhoneNumber= "9191919191",
                       Role=UserRoles.User,
                       Balance=1000m,
                       CreatedAt= DateTime.UtcNow,
                       UpdatedAt= DateTime.UtcNow,
                       IsDeleted= false
                    }
                }.AsQueryable();

            var mockContext = BuildMockContext(data);

            var repository = new UserRepository(mockContext.Object);
            var result = await repository.FindUserByEmailAsync(requestUserEmail);

            Assert.That(result, Is.True);
        }

        /// <summary>
        /// FindUserByEmailAsync Function - Did not Find user - Returns False
        /// </summary>
        /// <returns>false</returns>
        [Test]
        public async Task FindUserByEmailAsync_ReturnsFalse()
        {
            var requestUserEmail = "user@email.com";

            var data = new List<User>
                {
                    new User
                    {
                       UserId= Guid.NewGuid(),
                       Email="WrongEmail@gmail.com",
                       Password= "Password",
                       Name="Name",
                       PhoneNumber= "9191919191",
                       Role=UserRoles.User,
                       Balance=1000m,
                       CreatedAt= DateTime.UtcNow,
                       UpdatedAt= DateTime.UtcNow,
                       IsDeleted= false
                    }
                }.AsQueryable();

            var mockContext = BuildMockContext(data);

            var repository = new UserRepository(mockContext.Object);
            var result = await repository.FindUserByEmailAsync(requestUserEmail);

            Assert.That(result, Is.False);
        }

        /// <summary>
        /// GetUserByIdAsync Function  Found user - Returns User
        /// </summary>
        /// <returns>User</returns>
        [Test]
        public async Task GetUserByIdAsync_ReturnsUser()
        {
            var requestUserId= Guid.NewGuid();

            var data = new List<User>
                {
                    new User
                    {
                       UserId= requestUserId,
                       Email="Email@gmail.com",
                       Password= "Password",
                       Name="Name",
                       PhoneNumber= "9191919191",
                       Role=UserRoles.User,
                       Balance=1000m,
                       CreatedAt= DateTime.UtcNow,
                       UpdatedAt= DateTime.UtcNow,
                       IsDeleted= false
                    }
                }.AsQueryable();

            var mockContext = BuildMockContext(data);

            var repository = new UserRepository(mockContext.Object);
            var result = await repository.GetUserByIdAsync(requestUserId);

            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// GetUserByIdAsync Function - Did not Find user - Returns null
        /// </summary>
        /// <returns>null</returns>
        [Test]
        public async Task GetUserByIdAsync_ReturnsNull()
        {
            var requestUserId = Guid.NewGuid();

            var data = new List<User>
                {
                    new User
                    {
                       UserId= Guid.NewGuid(),
                       Email="Email@gmail.com",
                       Password= "Password",
                       Name="Name",
                       PhoneNumber= "9191919191",
                       Role=UserRoles.User,
                       Balance=1000m,
                       CreatedAt= DateTime.UtcNow,
                       UpdatedAt= DateTime.UtcNow,
                       IsDeleted= false
                    }
                }.AsQueryable();

            var mockContext = BuildMockContext(data);

            var repository = new UserRepository(mockContext.Object);
            var result = await repository.GetUserByIdAsync(requestUserId);

            Assert.That(result, Is.Null);
        }

        /// <summary>
        /// AddUser Function - Adds User Successfully
        /// </summary>
        [Test]
        public async Task AddUser_AddsUserCorrectly()
        {
            var mockSet = new Mock<DbSet<User>>();
            var mockContext = new Mock<AppDbContext>();

            mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            var repository = new UserRepository(mockContext.Object);

            var data =
                    new User
                    {
                        UserId = Guid.NewGuid(),
                        Email = "Email@gmail.com",
                        Password = "Password",
                        Name = "Name",
                        PhoneNumber = "9191919191",
                        Role = UserRoles.User,
                        Balance = 1000m,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };

            repository.AddUser(data);

            mockSet.Verify(m => m.Add(data), Times.Once);
        }

        public Mock<AppDbContext> BuildMockContext(IQueryable<User> data)
        {
            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IDbAsyncEnumerable<User>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<User>(data.GetEnumerator()));

            mockSet.As<IQueryable<User>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<User>(data.Provider));

            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            var mockContext = new Mock<AppDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            return mockContext;
        }

    }
}
