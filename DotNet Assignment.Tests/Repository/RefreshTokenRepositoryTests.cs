using DotNet_Assignment.Data;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Models.Enums;
using DotNet_Assignment.Repository.Address;
using DotNet_Assignment.Repository.RefreshTokens;
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
    public class RefreshTokenRepositoryTests
    {
        [Test]
        public async Task GetRefreshTokenAsync_ReturnsToken()
        {
            var requestRefreshToken = "RefreshToken";

            var data = new List<RefreshToken>
                {
                    new RefreshToken
                    {
                       RefreshTokenId= Guid.NewGuid(),
                       Token="RefreshToken",
                       ExpiresAt= DateTime.UtcNow.AddDays(7),
                       CreatedAt= DateTime.UtcNow,
                       UserId= Guid.NewGuid(),
                    }
                }.AsQueryable();

            var mockContext = BuildMockContext(data);

            var repository = new RefreshTokenRepository(mockContext.Object);
            var result = await repository.GetRefreshTokenAsync(requestRefreshToken);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetRefreshTokenAsync_ReturnsNull()
        {
            var requestRefreshToken = "RefreshToken";

            var data = new List<RefreshToken>
                {
                    new RefreshToken
                    {
                       RefreshTokenId= Guid.NewGuid(),
                       Token="WrongRefreshToken",
                       ExpiresAt= DateTime.UtcNow.AddDays(7),
                       CreatedAt= DateTime.UtcNow,
                       UserId= Guid.NewGuid(),
                    }
                }.AsQueryable();

            var mockContext = BuildMockContext(data);

            var repository = new RefreshTokenRepository(mockContext.Object);
            var result = await repository.GetRefreshTokenAsync(requestRefreshToken);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task AddRefreshToken_AddsCorrectly()
        {
            var mockSet = new Mock<DbSet<RefreshToken>>();
            var mockContext = new Mock<AppDbContext>();
            var requestRefreshToken = "RefreshToken";

            mockContext.Setup(c => c.RefreshTokens).Returns(mockSet.Object);

            var repository = new RefreshTokenRepository(mockContext.Object);

            var data =
                    new RefreshToken
                    {
                        RefreshTokenId = Guid.NewGuid(),
                        Token = "RefreshToken",
                        ExpiresAt = DateTime.UtcNow.AddDays(7),
                        CreatedAt = DateTime.UtcNow,
                        UserId = Guid.NewGuid(),
                    };

            repository.AddRefreshToken(data);

            mockSet.Verify(m => m.Add(data), Times.Once);
        }

        [Test]
        public async Task DeleteRefreshToken_DeletesCorrectly()
        {
            var requestRefreshToken = "RefreshToken";

            var data = new List<RefreshToken>
                {
                    new RefreshToken
                    {
                       RefreshTokenId= Guid.NewGuid(),
                       Token="RefreshToken",
                       ExpiresAt= DateTime.UtcNow.AddDays(7),
                       CreatedAt= DateTime.UtcNow,
                       UserId= Guid.NewGuid(),
                    }
                }.AsQueryable();

            var mockContext = BuildMockContext(data);

            var repository = new RefreshTokenRepository(mockContext.Object);
            var result = await repository.GetRefreshTokenAsync(requestRefreshToken);

            repository.DeleteRefreshToken(result);

            var mockSet = Mock.Get(mockContext.Object.RefreshTokens);
            mockSet.Verify(m => m.Remove(result), Times.Once);
        }

        [Test]
        public void DeleteTokensByUserId_DeletesAllUserTokens()
        {
            var requestUserId = Guid.NewGuid();

            var data = new List<RefreshToken>
                {
                    new RefreshToken { RefreshTokenId = Guid.NewGuid(), Token = "Token1", UserId = requestUserId },
                    new RefreshToken { RefreshTokenId = Guid.NewGuid(), Token = "Token2", UserId = requestUserId },
                }.AsQueryable();

            var mockContext = BuildMockContext(data);
            var repository = new RefreshTokenRepository(mockContext.Object);

            repository.DeleteTokensByUserId(requestUserId);

            var mockSet = Mock.Get(mockContext.Object.RefreshTokens);

            mockSet.Verify(m => m.RemoveRange(data), Times.Once);
        }

        public Mock<AppDbContext> BuildMockContext(IQueryable<RefreshToken> data)
        {
            var mockSet = new Mock<DbSet<RefreshToken>>();
            mockSet.As<IDbAsyncEnumerable<RefreshToken>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<RefreshToken>(data.GetEnumerator()));

            mockSet.As<IQueryable<RefreshToken>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<RefreshToken>(data.Provider));

            mockSet.As<IQueryable<RefreshToken>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<RefreshToken>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<RefreshToken>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            var mockContext = new Mock<AppDbContext>();
            mockContext.Setup(c => c.RefreshTokens).Returns(mockSet.Object);

            return mockContext;
        }
    }
}
