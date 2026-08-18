using DotNet_Assignment.Constants;
using DotNet_Assignment.Data;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Models.Enums;
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
    public class AddRestaurantOwnerTests
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
        /// AddRestaurantOwner function - User Not Found - Throws Exception
        /// </summary>
        [Test]
        public void AddRestaurantOwner_UserNotFound_ThrowsException()
        {
            var request = new RestaurantOwnerRequestDto
            {
                UserEmail = "owner@gmail.com",
                RestaurantId = Guid.NewGuid()
            };

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(request.UserEmail))
                .ReturnsAsync((User)null);

            Func<Task> action = async () => await _restaurantService.AddRestaurantOwner(request);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.UserNotFound));

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        /// <summary>
        /// AddRestaurantOwner function - User Deactivated - Throws Exception
        /// </summary>
        [Test]
        public void AddRestaurantOwner_UserDeactivated_ThrowsException()
        {
            var request = new RestaurantOwnerRequestDto
            {
                UserEmail = "owner@gmail.com",
                RestaurantId = Guid.NewGuid()
            };

            var user = new User
            {
                UserId = Guid.NewGuid(),
                IsDeleted = true
            };

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(request.UserEmail))
                .ReturnsAsync(user);

            Func<Task> action = async () => await _restaurantService.AddRestaurantOwner(request);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.UserDeactivated));

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        /// <summary>
        /// AddRestaurantOwner function - Restaurant Not Found - Throws Exception
        /// </summary>
        [Test]
        public void AddRestaurantOwner_RestaurantNotFound_ThrowsException()
        {
            var request = new RestaurantOwnerRequestDto
            {
                UserEmail = "owner@gmail.com",
                RestaurantId = Guid.NewGuid()
            };

            var user = new User
            {
                UserId = Guid.NewGuid(),
                IsDeleted = false,
                Role = UserRoles.User
            };

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(request.UserEmail))
                .ReturnsAsync(user);

            _restaurantRepository
                .Setup(x => x.GetRestaurantByIdAsync(request.RestaurantId))
                .ReturnsAsync((Restaurant)null);

            Func<Task> action = async () =>
                await _restaurantService.AddRestaurantOwner(request);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.RestaurantNotFound));

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        /// <summary>
        /// AddRestaurantOwner function - Role is user - Changes to owner
        /// </summary>
        [Test]
        public async Task AddRestaurantOwner_RoleIsUser_ChangesRoleToOwner()
        {
            var request = new RestaurantOwnerRequestDto
            {
                UserEmail = "owner@gmail.com",
                RestaurantId = Guid.NewGuid()
            };

            var user = new User
            {
                UserId = Guid.NewGuid(),
                IsDeleted = false,
                Role = UserRoles.User
            };

            var restaurant = new Restaurant
            {
                RestaurantId = request.RestaurantId
            };

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(request.UserEmail))
                .ReturnsAsync(user);

            _restaurantRepository
                .Setup(x => x.GetRestaurantByIdAsync(request.RestaurantId))
                .ReturnsAsync(restaurant);

            await _restaurantService.AddRestaurantOwner(request);

            Assert.That(user.Role, Is.EqualTo(UserRoles.Owner));
        }

        /// <summary>
        /// AddRestaurantOwner function - Role is admin - remains admin
        /// </summary>
        [Test]
        public async Task AddRestaurantOwner_RoleIsAdmin_DoesNotChange()
        {
            var request = new RestaurantOwnerRequestDto
            {
                UserEmail = "owner@gmail.com",
                RestaurantId = Guid.NewGuid()
            };

            var user = new User
            {
                UserId = Guid.NewGuid(),
                IsDeleted = false,
                Role = UserRoles.Admin
            };

            var restaurant = new Restaurant
            {
                RestaurantId = request.RestaurantId
            };

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(request.UserEmail))
                .ReturnsAsync(user);

            _restaurantRepository
                .Setup(x => x.GetRestaurantByIdAsync(request.RestaurantId))
                .ReturnsAsync(restaurant);

            await _restaurantService.AddRestaurantOwner(request);

            Assert.That(user.Role, Is.EqualTo(UserRoles.Admin));
        }

        /// <summary>
        /// AddRestaurantOwner function - Valid Request - Adds Owner
        /// </summary>
        [Test]
        public async Task AddRestaurantOwner_ValidRequest_AddsRestaurantOwner()
        {
            var request = new RestaurantOwnerRequestDto
            {
                UserEmail = "owner@gmail.com",
                RestaurantId = Guid.NewGuid()
            };

            var user = new User
            {
                UserId = Guid.NewGuid(),
                IsDeleted = false,
                Role = UserRoles.Owner
            };

            var restaurant = new Restaurant
            {
                RestaurantId = request.RestaurantId
            };

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(request.UserEmail))
                .ReturnsAsync(user);

            _restaurantRepository
                .Setup(x => x.GetRestaurantByIdAsync(request.RestaurantId))
                .ReturnsAsync(restaurant);

            await _restaurantService.AddRestaurantOwner(request);

            _restaurantRepository.Verify(x => x.AddRestaurantOwner(
                    It.Is<RestaurantOwner>(ro =>ro.RestaurantId == request.RestaurantId &&
                        ro.UserId == user.UserId)),Times.Once);
        }

        /// <summary>
        /// AddRestaurantOwner function - Valid request - saves Changes
        /// </summary>
        [Test]
        public async Task AddRestaurantOwner_ValidRequest_SavesChanges()
        {
            var request = new RestaurantOwnerRequestDto
            {
                UserEmail = "owner@gmail.com",
                RestaurantId = Guid.NewGuid()
            };

            var user = new User
            {
                UserId = Guid.NewGuid(),
                IsDeleted = false,
                Role = UserRoles.Owner
            };

            var restaurant = new Restaurant
            {
                RestaurantId = request.RestaurantId
            };

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(request.UserEmail))
                .ReturnsAsync(user);

            _restaurantRepository
                .Setup(x => x.GetRestaurantByIdAsync(request.RestaurantId))
                .ReturnsAsync(restaurant);

            await _restaurantService.AddRestaurantOwner(request);

            _appDbContext.Verify(x => x.SaveChangesAsync(),Times.Once);
        }
    }
}
