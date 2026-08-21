using DotNet_Assignment.Constants;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.Restaurants;
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
        /// GetAllRestaurants Function - Valid Request - Restaurants Found - Returns all restaurant
        /// </summary>
        [Test]
        public async Task GetAllRestaurants_RestaurantsFound_ReturnsRestaurants()
        {
            var page = 1;
            var pageSize = 10;

            var restaurants = RestaurantTestUtil.MockRestaurantHelper(Guid.NewGuid());

            _restaurantRepository.Setup(x => x.GetPagedRestaurantsAsync(page, pageSize)).ReturnsAsync(restaurants);

            var result = await _restaurantService.GetAllRestaurantsAsync(page, pageSize);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// GetAllRestaurants Function - Valid Request - No Restaurants Found - Returns Empty list
        /// </summary>
        [Test]
        public async Task GetAllRestaurants_NoRestaurants_ReturnsEmpty()
        {
            var page = 1;
            var pageSize = 10;

            _restaurantRepository.Setup(x => x.GetPagedRestaurantsAsync(page, pageSize)).ReturnsAsync(new List<Restaurant>());

            var response = await _restaurantService.GetAllRestaurantsAsync(page, pageSize);

            Assert.That(response, Is.Not.Null);
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

            _restaurantRepository.Setup(x => x.GetPagedRestaurantsAsync(page, pageSize)).ReturnsAsync(restaurants);

            await _restaurantService.GetAllRestaurantsAsync(page, pageSize);

            _restaurantRepository.Verify(x => x.GetPagedRestaurantsAsync(page, pageSize), Times.Once);
        }

        /// <summary>
        /// GetAllRestaurants Function - Valid Request -Multiple Restaurants Found - Returns all restaurants
        /// </summary>
        [Test]
        public async Task GetAllRestaurants_MultipleRestaurants_ReturnsAllRestaurants()
        {
            var restaurants = RestaurantTestUtil.MockRestaurantHelper(Guid.NewGuid());

            _restaurantRepository.Setup(x => x.GetPagedRestaurantsAsync(1, 10)).ReturnsAsync(restaurants);

            var result = await _restaurantService.GetAllRestaurantsAsync(1, 10);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("Restaurant"));
        }
    }
}
