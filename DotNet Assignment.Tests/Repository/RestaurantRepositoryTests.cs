using DotNet_Assignment.Data;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.Restaurants;
using DotNet_Assignment.Tests.Utils.RepositoryHelpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet_Assignment.Tests.Repository
{
    [TestFixture]
    public class RestaurantRepositoryTests
    {
        /// <summary>
        /// GetRestaurantsAsync Function - Restaurants found - Returns restaurants successfully
        /// </summary>
        /// <returns>List of restaurants</returns>
        [Test]
        public async Task GetRestaurantsAsync_RestaurantsFound_ReturnsRestaurants()
        {
            var data = new List<Restaurant>
            {
                new Restaurant
                {
                    RestaurantId = Guid.NewGuid(),
                    Name = "Restaurant",
                    Rating = 5
                },

            }.AsQueryable();

            var mockContext = BuildRestaurantMockContext(data);

            var repository = new RestaurantRepository(mockContext.Object);

            var result = await repository.GetPagedRestaurantsAsync(1, 10);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// GetRestaurantsAsync Function - No restaurants found - Returns empty list
        /// </summary>
        /// <returns>Empty list</returns>
        [Test]
        public async Task GetRestaurantsAsync_NoRestaurantsFound_ReturnsEmptyList()
        {
            var data = new List<Restaurant>().AsQueryable();

            var mockContext = BuildRestaurantMockContext(data);

            var repository = new RestaurantRepository(mockContext.Object);

            var result = await repository.GetPagedRestaurantsAsync(1, 10);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        /// <summary>
        /// GetRestaurantByIdAsync Function - Restaurant found - Returns restaurant successfully
        /// </summary>
        /// <returns>Restaurant</returns>
        [Test]
        public async Task GetRestaurantByIdAsync_RestaurantFound_ReturnsRestaurant()
        {
            var requestRestaurantId = Guid.NewGuid();

            var data = new List<Restaurant>
            {
                new Restaurant
                {
                    RestaurantId = requestRestaurantId,
                    Name = "Restaurant",
                    Rating = 5
                }
            }.AsQueryable();

            var mockContext = BuildRestaurantMockContext(data);

            var repository = new RestaurantRepository(mockContext.Object);

            var result =
                await repository.GetRestaurantByIdAsync(requestRestaurantId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RestaurantId, Is.EqualTo(requestRestaurantId));
        }

        /// <summary>
        /// GetRestaurantByIdAsync Function - Restaurant not found - Returns null
        /// </summary>
        /// <returns>null</returns>
        [Test]
        public async Task GetRestaurantByIdAsync_RestaurantNotFound_ReturnsNull()
        {
            var requestRestaurantId = Guid.NewGuid();

            var data = new List<Restaurant>
            {
                new Restaurant
                {
                    RestaurantId = Guid.NewGuid(),
                    Name = "Restaurant",
                    Rating = 5
                }
            }.AsQueryable();

            var mockContext = BuildRestaurantMockContext(data);

            var repository = new RestaurantRepository(mockContext.Object);

            var result =
                await repository.GetRestaurantByIdAsync(requestRestaurantId);

            Assert.That(result, Is.Null);
        }

        /// <summary>
        /// GetMenuItemByIdAsync Function - Menu item found - Returns menu item successfully
        /// </summary>
        /// <returns>Menu item</returns>
        [Test]
        public async Task GetMenuItemByIdAsync_MenuItemFound_ReturnsMenuItem()
        {
            var requestMenuItemId = Guid.NewGuid();

            var data = new List<MenuItem>
            {
                new MenuItem
                {
                    MenuItemId = requestMenuItemId,
                    Name = "Burger"
                }
            }.AsQueryable();

            var mockContext = BuildMenuItemMockContext(data);

            var repository = new RestaurantRepository(mockContext.Object);

            var result = await repository.GetMenuItemByIdAsync(requestMenuItemId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.MenuItemId, Is.EqualTo(requestMenuItemId));
        }

        /// <summary>
        /// GetMenuItemByIdAsync Function - Menu item not found - Returns null
        /// </summary>
        /// <returns>null</returns>
        [Test]
        public async Task GetMenuItemByIdAsync_MenuItemNotFound_ReturnsNull()
        {
            var requestMenuItemId = Guid.NewGuid();

            var data = new List<MenuItem>
            {
                new MenuItem
                {
                    MenuItemId = Guid.NewGuid(),
                    Name = "Burger"
                }
            }.AsQueryable();

            var mockContext = BuildMenuItemMockContext(data);

            var repository = new RestaurantRepository(mockContext.Object);

            var result = await repository.GetMenuItemByIdAsync(requestMenuItemId);

            Assert.That(result, Is.Null);
        }

        /// <summary>
        /// AddRestaurant Function - Adds restaurant successfully
        /// </summary>
        [Test]
        public void AddRestaurant_AddsRestaurantSuccessfully()
        {
            var mockSet = new Mock<DbSet<Restaurant>>();
            var mockContext = new Mock<AppDbContext>();

            mockContext
                .Setup(x => x.Restaurants)
                .Returns(mockSet.Object);

            var repository = new RestaurantRepository(mockContext.Object);

            var restaurant = new Restaurant
            {
                RestaurantId = Guid.NewGuid(),
                Name = "Restaurant"
            };

            repository.AddRestaurant(restaurant);

            mockSet.Verify(x => x.Add(restaurant), Times.Once);
        }

        /// <summary>
        /// AddRestaurantOwner Function - Adds restaurant owner successfully
        /// </summary>
        [Test]
        public void AddRestaurantOwner_AddsOwnerSuccessfully()
        {
            var mockSet = new Mock<DbSet<RestaurantOwner>>();
            var mockContext = new Mock<AppDbContext>();

            mockContext
                .Setup(x => x.RestaurantsOwner)
                .Returns(mockSet.Object);

            var repository = new RestaurantRepository(mockContext.Object);

            var restaurantOwner = new RestaurantOwner
            {
                RestaurantId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            repository.AddRestaurantOwner(restaurantOwner);

            mockSet.Verify(x => x.Add(restaurantOwner), Times.Once);
        }

        private Mock<AppDbContext> BuildRestaurantMockContext(IQueryable<Restaurant> data)
        {
            var mockSet = new Mock<DbSet<Restaurant>>();
            mockSet.As<IDbAsyncEnumerable<Restaurant>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<Restaurant>(data.GetEnumerator()));

            mockSet.As<IQueryable<Restaurant>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<Restaurant>(data.Provider));

            mockSet.As<IQueryable<Restaurant>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Restaurant>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Restaurant>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockSet.Setup(x => x.AsNoTracking()).Returns(mockSet.Object);

            var mockContext = new Mock<AppDbContext>();
            mockContext.Setup(c => c.Restaurants).Returns(mockSet.Object);

            return mockContext;
        }

        private Mock<AppDbContext> BuildMenuItemMockContext(IQueryable<MenuItem> data)
        {
            var mockSet = new Mock<DbSet<MenuItem>>();
            mockSet.As<IDbAsyncEnumerable<MenuItem>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<MenuItem>(data.GetEnumerator()));

            mockSet.As<IQueryable<MenuItem>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<MenuItem>(data.Provider));

            mockSet.As<IQueryable<MenuItem>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<MenuItem>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<MenuItem>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockSet.Setup(x => x.AsNoTracking()).Returns(mockSet.Object);

            var mockContext = new Mock<AppDbContext>();
            mockContext.Setup(c => c.MenuItems).Returns(mockSet.Object);

            return mockContext;
        }
    }
}
