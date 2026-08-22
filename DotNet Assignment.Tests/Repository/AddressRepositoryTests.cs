using DotNet_Assignment.Data;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.Address;
using DotNet_Assignment.Tests.Utils.RepositoryHelpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Assignment.Tests.Repository 
{ 
    [TestFixture]
    public class AddressRepositoryTests
    {
        /// <summary>
        /// GetAddressByIdAsync Function - Address Found - Returns address successfully
        /// </summary>
        /// <returns>User Address</returns>
        [Test]
        public async Task GetAddressByIdAsync_AddressFound_ReturnsAddress()
        {
            var requestAddressId = Guid.NewGuid();
            var requestUserId = Guid.NewGuid();

            var data = new List<UserAddress>
                {
                    new UserAddress
                    {
                        UserAddressId= requestAddressId,
                        HouseNumber="10",
                        Street= "street",
                        Landmark= "Landmark",
                        City= "City",
                        State= "State",
                        Pincode= "111111",
                        CreatedAt= DateTime.UtcNow,
                        UpdatedAt= DateTime.UtcNow,
                        UserId= requestUserId,
                    }
                }.AsQueryable();

            var mockContext = BuildMockContext(data);

            var repository = new AddressRepository(mockContext.Object);
            var result = await repository.GetAddressByIdAsync(requestAddressId, requestUserId);

            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// GetAddressByIdAsync Function - Address not found - returns Null
        /// </summary>
        /// <returns>null</returns>
        [Test]
        public async Task GetAddressByIdAsync_AddressNotFound_ReturnsNull()
        {
            var requestAddressId = Guid.NewGuid();
            var requestUserId = Guid.NewGuid();

            var data = new List<UserAddress>
                {
                    new UserAddress
                    {
                        UserAddressId= Guid.NewGuid(),
                        HouseNumber="10",
                        Street= "street",
                        Landmark= "Landmark",
                        City= "City",
                        State= "State",
                        Pincode= "111111",
                        CreatedAt= DateTime.UtcNow,
                        UpdatedAt= DateTime.UtcNow,
                        UserId= requestUserId,
                    }
                }.AsQueryable();

            var mockContext = BuildMockContext(data);

            var repository = new AddressRepository(mockContext.Object);
            var result = await repository.GetAddressByIdAsync(requestAddressId, requestUserId);

            Assert.That(result, Is.Null);
        }

        /// <summary>
        /// AddAddress Function - Adds address successfully
        /// </summary>
        [Test]
        public async Task AddAddress_AddsAddressSuccessfully()
        {
            var mockSet = new Mock<DbSet<UserAddress>>();
            var mockContext = new Mock<AppDbContext>();

            mockContext.Setup(c=> c.UserAddresses).Returns(mockSet.Object);

            var repository = new AddressRepository(mockContext.Object);

            var data =
                    new UserAddress
                    {
                        UserAddressId = Guid.NewGuid(),
                        HouseNumber = "10",
                        Street = "street",
                        Landmark = "Landmark",
                        City = "City",
                        State = "State",
                        Pincode = "111111",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        UserId = Guid.NewGuid(),
                    };

            repository.AddAddress(data);

            mockSet.Verify(m => m.Add(data), Times.Once);
        }

        private Mock<AppDbContext> BuildMockContext(IQueryable<UserAddress> data)
        {
            var mockSet = new Mock<DbSet<UserAddress>>();
            mockSet.As<IDbAsyncEnumerable<UserAddress>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<UserAddress>(data.GetEnumerator()));

            mockSet.As<IQueryable<UserAddress>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<UserAddress>(data.Provider));

            mockSet.As<IQueryable<UserAddress>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<UserAddress>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<UserAddress>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            var mockContext = new Mock<AppDbContext>();
            mockContext.Setup(c => c.UserAddresses).Returns(mockSet.Object);

            return mockContext;
        }
    }
}
