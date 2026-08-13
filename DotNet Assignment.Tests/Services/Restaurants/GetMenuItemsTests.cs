using DotNet_Assignment.Constants;
using DotNet_Assignment.Data;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.Address;
using DotNet_Assignment.Repository.Restaurants;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.Address;
using DotNet_Assignment.Services.Restaurants;
using DotNet_Assignment.Tests.Utils;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet_Assignment.Tests.Services.Restaurants
{
    [TestFixture]
    public class GetMenuItemsTests
    {
        private Mock<IRestaurantRepository> _restaurantRepository;
        private RestaurantService _restaurantService;

        [SetUp]
        public void Setup()
        {
            _restaurantRepository = new Mock<IRestaurantRepository>();

            _restaurantService = new RestaurantService(
                _restaurantRepository.Object
            );
        }

        /// <summary>
        /// GetMenuItemsByRestaurantId Function - Valid Request - Restraurant Found - Returns Menu Items
        /// </summary>
        [Test]
        public async Task GetMenuItemsByRestaurantId_RestaurantFound_ReturnsMenuItems()
        {
            var restaurantId = Guid.NewGuid();

            var restaurant = new Restaurant
            {
                RestaurantId = restaurantId,
                MenuItems = RestaurantTestUtil.MockMenuItemHelper(Guid.NewGuid())
            };

            _restaurantRepository
                .Setup(x => x.GetRestaurantByIdAsync(restaurantId))
                .ReturnsAsync(restaurant);

            var result = await _restaurantService.GetMenuItemsByRestaurantIdAsync(restaurantId, 1, 10);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// GetMenuItemsByRestaurantId Function - Restaurant Not Found - Throws Exception
        /// </summary>
        [Test]
        public void GetMenuItemsByRestaurantId_RestaurantNotFound_ThrowsException()
        {
            var restaurantId = Guid.NewGuid();

            _restaurantRepository
                .Setup(x => x.GetRestaurantByIdAsync(restaurantId))
                .ReturnsAsync((Restaurant)null);

            Func<Task> Action = async () => await _restaurantService.GetMenuItemsByRestaurantIdAsync(restaurantId, 1, 10);
            var exception = Assert.CatchAsync<Exception>(Action);

            Assert.That(exception.Message,Is.EqualTo(ExceptionMessages.RestaurantNotFound));
        }

        /// <summary>
        /// GetMenuItemsByRestaurantId Function - Valid Request - Calls Repository function
        /// </summary>
        [Test]
        public void GetMenuItemsByRestaurantId_NoMenuItems_ThrowsException()
        {
            var restaurantId = Guid.NewGuid();

            var restaurant = new Restaurant
            {
                RestaurantId = restaurantId,
                MenuItems = new List<MenuItem>()
            };

            _restaurantRepository
                .Setup(x => x.GetRestaurantByIdAsync(restaurantId))
                .ReturnsAsync(restaurant);


            Func<Task> Action = async () => await _restaurantService.GetMenuItemsByRestaurantIdAsync(restaurantId, 1, 10);
            var exception = Assert.CatchAsync<Exception>(Action);

            Assert.That( exception.Message, Is.EqualTo(ExceptionMessages.NoMenuItems));
        }

        /// <summary>
        /// GetMenuItemsByRestaurantId Function - Valid Request - Calls Repository function
        /// </summary>
        [Test]
        public async Task GetMenuItemsByRestaurantId_ValidRequest_CallsRepository()
        {
            var restaurantId = Guid.NewGuid();

            var restaurant = new Restaurant
            {
                RestaurantId = restaurantId,
                MenuItems = RestaurantTestUtil.MockMenuItemHelper(Guid.NewGuid())
            };

            _restaurantRepository
                .Setup(x => x.GetRestaurantByIdAsync(restaurantId))
                .ReturnsAsync(restaurant);

            await _restaurantService.GetMenuItemsByRestaurantIdAsync(restaurantId, 1, 10);

            _restaurantRepository.Verify(x => x.GetRestaurantByIdAsync(restaurantId), Times.Once);
        }
    }
}
