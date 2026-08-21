using DotNet_Assignment.Constants;
using DotNet_Assignment.Controllers;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Services.Restaurants;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Results;

namespace DotNet_Assignment.Tests.Controllers.Restaurant
{
    [TestFixture]
    public class RestaurantControllerTests
    {
        private Mock<IRestaurantService> _restaurantService;
        private RestaurantController _restaurantController;

        [SetUp]
        public void Setup()
        {
            _restaurantService = new Mock<IRestaurantService>();
            _restaurantController = new RestaurantController(_restaurantService.Object);
        }

        /// <summary>
        /// GetRestaurants - Valid Request - Returns success
        /// </summary>
        [Test]
        public async Task GetRestaurants_ValidRequest_ReturnsSuccess()
        {
            var page = 1;
            var pageSize = 10;
            var data = new List<RestaurantResponseDto>();

            _restaurantService
                .Setup(x => x.GetAllRestaurantsAsync(page, pageSize))
                .ReturnsAsync(data);

            var result = await _restaurantController.GetRestaurantsAsync(page, pageSize);

            var okResult = result as OkNegotiatedContentResult<ApiResponseDto<List<RestaurantResponseDto>>>;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Content.IsSuccess, Is.True);
            Assert.That(okResult.Content.Data, Is.EqualTo(data));
        }

        /// <summary>
        /// GetRestaurants - Service Throws Exception - Throws Exception
        /// </summary>
        [Test]
        public void GetRestaurants_ServiceThrowsException_ThrowsException()
        {
            var page = 1;
            var pageSize = 10;

            _restaurantService
                .Setup(x => x.GetAllRestaurantsAsync(page, pageSize))
                .ThrowsAsync(new Exception("Restaurants not found"));

            Func<Task> action = async () => await _restaurantController.GetRestaurantsAsync(page, pageSize);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo("Restaurants not found"));
        }

        /// <summary>
        /// GetMenuItems function - Valid Request - Returns Success
        /// </summary>
        [Test]
        public async Task GetMenuItems_ValidRequest_ReturnsSuccess()
        {
            var restaurantId = Guid.NewGuid();
            var page = 1;
            var pageSize = 10;
            var data = new List<MenuItemResponseDto>();

            _restaurantService.Setup(x => x.GetMenuItemsByRestaurantIdAsync(restaurantId, page, pageSize)).ReturnsAsync(data);

            var result = await _restaurantController.GetMenuItemsAsync(restaurantId, page, pageSize);

            var okResult = result as OkNegotiatedContentResult<ApiResponseDto<List<MenuItemResponseDto>>>;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Content.IsSuccess, Is.True);
            Assert.That(okResult.Content.Data, Is.EqualTo(data));
        }

        /// <summary>
        /// GetMenuItems - Service Throws Exception - Throws Exception
        /// </summary>
        [Test]
        public void GetMenuItems_ServiceThrowsException_ThrowsException()
        {
            var restaurantId = Guid.NewGuid();
            var page = 1;
            var pageSize = 10;

            _restaurantService.Setup(x => x.GetMenuItemsByRestaurantIdAsync(restaurantId, page, pageSize)).ThrowsAsync(new Exception(ExceptionMessages.RestaurantNotFound));

            Func<Task> action = async () => await _restaurantController.GetMenuItemsAsync(restaurantId, page, pageSize);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.RestaurantNotFound));
        }

        /// <summary>
        /// AddRestaurant function - Valid Request - return success
        /// </summary>
        [Test]
        public async Task AddRestaurant_ValidRequest_ReturnsSuccess()
        {
            var request = new AddRestaurantDto
            {
                Name = "Restaurant",
                UserEmail = "owner@gmail.com"
            };

            var result = await _restaurantController.AddRestaurantAsync(request);

            _restaurantService.Verify(x => x.AddRestaurantAsync(request),Times.Once);

            Assert.That(result,Is.TypeOf<OkNegotiatedContentResult<ApiResponseDto<List<MenuItemResponseDto>>>>());
        }

        /// <summary>
        /// AddRestaurant function - Service throws - throws exception
        /// </summary>
        [Test]
        public void AddRestaurant_ServiceThrows_ThrowsException()
        {
            var request = new AddRestaurantDto
            {
                Name = "Restaurant",
                UserEmail = "owner@gmail.com"
            };

            _restaurantService
                .Setup(x => x.AddRestaurantAsync(request))
                .ThrowsAsync(new Exception(ExceptionMessages.UserNotFound));

            Func<Task> action = async () => await _restaurantController.AddRestaurantAsync(request);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.UserNotFound));
        }

        /// <summary>
        /// AddRestaurantOwner function - Valid Request - return success
        /// </summary>
        [Test]
        public async Task AddRestaurantOwner_ValidRequest_ReturnsSuccess()
        {
            var request = new RestaurantOwnerRequestDto
            {
                RestaurantId = Guid.NewGuid(),
                UserEmail = "owner@gmail.com"
            };

            var result = await _restaurantController.AddRestaurantOwnerAsync(request);

            _restaurantService.Verify( x => x.AddRestaurantOwner(request), Times.Once);

            Assert.That( result, Is.TypeOf<OkNegotiatedContentResult<ApiResponseDto<List<MenuItemResponseDto>>>>());
        }

        /// <summary>
        /// AddRestaurantOwner function - Service throws - throws exception
        /// </summary>
        [Test]
        public void AddRestaurantOwner_ServiceThrows_ThrowsException()
        {
            var request = new RestaurantOwnerRequestDto
            {
                RestaurantId = Guid.NewGuid(),
                UserEmail = "owner@gmail.com"
            };

            _restaurantService
                .Setup(x => x.AddRestaurantOwner(request))
                .ThrowsAsync(new Exception(ExceptionMessages.UserNotFound));

            Func<Task> action = async () => await _restaurantController.AddRestaurantOwnerAsync(request);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.UserNotFound));
        }
    }
}
