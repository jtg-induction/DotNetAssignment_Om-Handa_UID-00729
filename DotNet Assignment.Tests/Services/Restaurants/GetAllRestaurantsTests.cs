using DotNet_Assignment.Constants;
using DotNet_Assignment.Data;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.Restaurants;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.Restaurants;
using DotNet_Assignment.Tests.Utils;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Assignment.Tests.Services.Restaurants
{
    [TestFixture]
    public class GetAllRestaurantsTests
    {
        private Mock<IRestaurantRepository> _restaurantRepository;
        private Mock<IUserRepository> _userRepository;
        private Mock<AppDbContext> _appDbContext;

        private RestaurantService _restaurantService;

        [SetUp]
        public void Setup()
        {
            _restaurantRepository = new Mock<IRestaurantRepository>();
            _userRepository = new Mock<IUserRepository>();
            _appDbContext = new Mock<AppDbContext>();

            _restaurantService = new RestaurantService(
                _restaurantRepository.Object,
                _userRepository.Object,
                _appDbContext.Object
            );
        }

        /// <summary>
        /// GetAllRestaurants Function - Valid Request - Restaurants Found - Returns all restaurant
        /// </summary>
        [Test]
        public async Task GetAllRestaurants_RestaurantsFound_ReturnsRestaurants()
        {
            var page = 1;
            var pageSize = 10;

            var restaurants = RestaurantTestUtil.MockRestaurantHelper(Guid.NewGuid());

            _restaurantRepository.Setup(x => x.GetRestaurantsAsync(page, pageSize)).ReturnsAsync(restaurants);

            var result = await _restaurantService.GetAllRestaurantsAsync(page, pageSize);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// GetAllRestaurants Function - Valid Request - No Restaurants Found - Throws Exception
        /// </summary>
        [Test]
        public void GetAllRestaurants_NoRestaurants_ThrowsException()
        {
            var page = 1;
            var pageSize = 10;

            _restaurantRepository.Setup(x => x.GetRestaurantsAsync(page, pageSize)).ReturnsAsync(new List<Restaurant>());

            Func<Task> Action = async () => await _restaurantService.GetAllRestaurantsAsync(page, pageSize);
            var exception = Assert.CatchAsync<Exception>(Action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.NoRestaurants));
        }

        /// <summary>
        /// GetAllRestaurants Function - Valid Request - Restaurants Found - Throws No Exception
        /// </summary>
        [Test]
        public async Task GetAllRestaurants_ValidPagination_ThrowsNoException()
        {
            var page = 2;
            var pageSize = 5;

            var restaurants = RestaurantTestUtil.MockRestaurantHelper(Guid.NewGuid());

            _restaurantRepository.Setup(x => x.GetRestaurantsAsync(page, pageSize)).ReturnsAsync(restaurants);

            await _restaurantService.GetAllRestaurantsAsync(page, pageSize);

            _restaurantRepository.Verify(x => x.GetRestaurantsAsync(page, pageSize), Times.Once);
        }

        /// <summary>
        /// GetAllRestaurants Function - Valid Request -Multiple Restaurants Found - Returns all restaurants
        /// </summary>
        [Test]
        public async Task GetAllRestaurants_MultipleRestaurants_ReturnsAllRestaurants()
        {
            var restaurants = RestaurantTestUtil.MockRestaurantHelper(Guid.NewGuid());

            _restaurantRepository.Setup(x => x.GetRestaurantsAsync(1, 10)).ReturnsAsync(restaurants);

            var result = await _restaurantService.GetAllRestaurantsAsync(1, 10);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("Restaurant"));
        }
    }
}
