using DotNet_Assignment.Constants;
using DotNet_Assignment.Data;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.Restaurants;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.Restaurants;
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
    public class AddRestaurantTests
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
        /// AddRestaurant function - Valid Request - Adds Restaurant
        /// </summary>
        [Test]
        public async Task AddRestaurant_ValidRequest_AddsRestaurant()
        {
            var request = new AddRestaurantDto
            {
                Name = "Restaurant",
                Description = "Good Food",
                Street = "Street",
                Landmark = "Landmark",
                City = "City",
                State = "State",
                Pincode = "111111",
                UserEmail = "owner@gmail.com"
            };

            var user = new User
            {
                UserId = Guid.NewGuid()
            };

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(request.UserEmail))
                .ReturnsAsync(user);

            await _restaurantService.AddRestaurantAsync(request);

            _restaurantRepository.Verify(x => x.AddRestaurant(It.IsAny<Restaurant>()), Times.Once);
        }

        /// <summary>
        /// AddRestaurant function- Valid Request - Adds Restaurant Owner
        /// </summary>
        [Test]
        public async Task AddRestaurant_ValidRequest_AddsRestaurantOwner()
        {
            var request = new AddRestaurantDto
            {
                Name = "Restaurant",
                UserEmail = "owner@gmail.com",
                City = "City",
                Description = "Description",
                Landmark = "Landmark",
                Pincode = "147201",
                State = "State",
                Street = "street"
            };

            var user = new User
            {
                UserId = Guid.NewGuid(),
            };

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(request.UserEmail))
                .ReturnsAsync(user);

            await _restaurantService.AddRestaurantAsync(request);

            _restaurantRepository.Verify(x => x.AddRestaurantOwner(It.Is<RestaurantOwner>(r => r.UserId == user.UserId)), Times.Once);
        }

        /// <summary>
        /// AddRestaurant function - Valid Request - Saves Changes
        /// </summary>
        [Test]
        public async Task AddRestaurant_ValidRequest_SavesChanges()
        {
            var request = new AddRestaurantDto
            {
                Name = "Restaurant",
                UserEmail = "owner@gmail.com"
            };

            var user = new User
            {
                UserId = Guid.NewGuid()
            };

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(request.UserEmail))
                .ReturnsAsync(user);

            await _restaurantService.AddRestaurantAsync(request);

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// AddRestaurant function - User Not Found - Throws Exception
        /// </summary>
        [Test]
        public void AddRestaurant_UserNotFound_ThrowsException()
        {
            var request = new AddRestaurantDto
            {
                Name = "Restaurant",
                UserEmail = "owner@gmail.com"
            };

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(request.UserEmail))
                .ReturnsAsync((User)null);

            Func<Task> action = async () => await _restaurantService.AddRestaurantAsync(request);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.UserNotFound));

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        /// <summary>
        /// AddRestaurant function - User Deactivated - Throws Exception
        /// </summary>
        [Test]
        public void AddRestaurant_UserDeactivated_ThrowsException()
        {
            var request = new AddRestaurantDto
            {
                Name = "Restaurant",
                UserEmail = "owner@gmail.com"
            };

            var user = new User
            {
                UserId = Guid.NewGuid(),
                IsDeleted = true
            };

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(request.UserEmail))
                .ReturnsAsync(user);

            Func<Task> action = async () => await _restaurantService.AddRestaurantAsync(request);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.UserDeactivated));

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }
    }
}
