using DotNet_Assignment.Constants;
using DotNet_Assignment.Data;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.Address;
using DotNet_Assignment.Repository.Orders;
using DotNet_Assignment.Repository.Restaurants;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.Orders;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace DotNet_Assignment.Tests.Services.Orders
{
    [TestFixture]
    public class PlaceOrderTests
    {
        private Mock<IOrderRepository> _orderRepository;
        private Mock<IRestaurantRepository> _restaurantRepository;
        private Mock<IUserRepository> _userRepository;
        private Mock<IAddressRepository> _addressRepository;
        private Mock<AppDbContext> _appDbContext;
        private OrderService _orderService;

        [SetUp]
        public void Setup()
        {
            _orderRepository = new Mock<IOrderRepository>();
            _restaurantRepository = new Mock<IRestaurantRepository>();
            _userRepository = new Mock<IUserRepository>();
            _addressRepository = new Mock<IAddressRepository>();
            _appDbContext = new Mock<AppDbContext>();

            _orderService = new OrderService(
                _orderRepository.Object,
                _restaurantRepository.Object,
                _addressRepository.Object,
                _userRepository.Object,
                _appDbContext.Object);
        }

        /// <summary>
        /// PlacerOrder Function - Valid Request - Return Order Response
        /// </summary>
        [Test]
        public async Task PlaceOrder_ValidRequest_ReturnsOrderResponse()
        {
            var userId = Guid.NewGuid();
            var restaurantId = Guid.NewGuid();
            var addressId = Guid.NewGuid();
            var menuItemId = Guid.NewGuid();

            var user = new User
            {
                UserId = userId,
                IsDeleted = false,
                Balance = 1000
            };

            var menuItem = new MenuItem
            {
                MenuItemId = menuItemId,
                RestaurantId = restaurantId,
                Price = 200,
                QuantityAvailable=10
            };

            var restaurant = new Restaurant
            {
                RestaurantId = restaurantId,
                MenuItems = new List<MenuItem> { menuItem }
            };

            var address = new UserAddress
            {
                UserAddressId = addressId,
                UserId = userId,
                HouseNumber = "10",
                Street = "Street",
                City = "City",
                State = "State",
                Pincode = "111111",
                Landmark = "Landmark"
            };

            var request = new OrderRequestDto
            {
                RestaurantId = restaurantId,
                AddressId = addressId,
                OrderedItems = new List<OrderItemDto>
                    {
                        new OrderItemDto
                        {
                            MenuItemId = menuItemId,
                            Quantity = 2
                        }
                    }
            };

            _userRepository.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _restaurantRepository.Setup(x => x.GetRestaurantByIdAsync(restaurantId)).ReturnsAsync(restaurant);
            _addressRepository.Setup(x => x.GetAddressByIdAsync(addressId, userId)).ReturnsAsync(address);
            _restaurantRepository.Setup(x => x.GetMenuItemForUpdateAsync(menuItemId)).ReturnsAsync(menuItem);

            var result = await _orderService.ExecutePlaceOrderAsync(userId, request);

            Assert.That(result, Is.Not.Null);
            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// PlacerOrder Function - User Not Found - Throw Exception
        /// </summary>
        [Test]
        public void PlaceOrder_UserNotFound_ThrowsException()
        {
            var userId = Guid.NewGuid();

            _userRepository.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            Func<Task> Action = async () => await _orderService.ExecutePlaceOrderAsync(userId, new OrderRequestDto());
            var exception = Assert.CatchAsync<Exception>(Action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.UserNotFound));
            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        /// <summary>
        /// PlacerOrder Function - Restaurant Not Found - Throw Exception
        /// </summary>
        [Test]
        public void PlaceOrder_RestaurantNotFound_ThrowsException()
        {
            var userId = Guid.NewGuid();
            var restaurantId = Guid.NewGuid();

            var user = new User
            {
                UserId = userId,
                IsDeleted = false,
                Balance = 1000
            };

            var request = new OrderRequestDto
            {
                RestaurantId = restaurantId
            };

            _userRepository.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _restaurantRepository.Setup(x => x.GetRestaurantByIdAsync(restaurantId)).ReturnsAsync((Restaurant)null);

            Func<Task> Action = async () => await _orderService.ExecutePlaceOrderAsync(userId, new OrderRequestDto());
            var exception = Assert.CatchAsync<Exception>(Action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.RestaurantNotFound));
            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        /// <summary>
        /// PlacerOrder Function - Menu Items Not Found - Throw Exception
        /// </summary>
        [Test]
        public async Task PlaceOrder_MenuItemsNotFound_ReturnsEmpty()
        {
            var userId = Guid.NewGuid();
            var restaurantId = Guid.NewGuid();

            var user = new User
            {
                UserId = userId,
                IsDeleted = false,
                Balance = 1000
            };

            var restaurant = new Restaurant
            {
                RestaurantId = restaurantId,
                MenuItems = new List<MenuItem>()
            };

            var request = new OrderRequestDto
            {
                RestaurantId = restaurantId
            };

            _userRepository.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _restaurantRepository.Setup(x => x.GetRestaurantByIdAsync(restaurantId)).ReturnsAsync(restaurant);

            var response = await _orderService.ExecutePlaceOrderAsync(userId, request);

            Assert.That(response, Is.Not.Null);
            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// PlacerOrder Function - No ordered Items - Throw Exception
        /// </summary>
        [Test]
        public void PlaceOrder_NoOrderedItems_ThrowsException()
        {
            var userId = Guid.NewGuid();
            var restaurantId = Guid.NewGuid();
            var addressId = Guid.NewGuid();

            var user = new User
            {
                UserId = userId,
                IsDeleted = false,
                Balance = 1000
            };

            var restaurant = new Restaurant
            {
                RestaurantId = restaurantId,
                MenuItems = new List<MenuItem>
                {
                    new MenuItem()
                }
            };

            var request = new OrderRequestDto
            {
                RestaurantId = restaurantId,
                AddressId = addressId,
                OrderedItems = null
            };

            var address = new UserAddress
            {
                UserAddressId = addressId,
                UserId = userId
            };

            _userRepository.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _restaurantRepository.Setup(x => x.GetRestaurantByIdAsync(restaurantId)).ReturnsAsync(restaurant);
            _addressRepository.Setup(x => x.GetAddressByIdAsync(addressId, userId)).ReturnsAsync(address);

            Func<Task> Action = async () => await _orderService.ExecutePlaceOrderAsync(userId, request);
            var exception = Assert.CatchAsync<Exception>(Action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.AtleastOneOrderItemRequired));
            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        /// <summary>
        /// PlacerOrder Function - Insufficient Balance - Throw Exception
        /// </summary>
        [Test]
        public void PlaceOrder_InsufficientBalance_ThrowsException()
        {
            var userId = Guid.NewGuid();
            var restaurantId = Guid.NewGuid();
            var menuItemId = Guid.NewGuid();
            var addressId = Guid.NewGuid();

            var user = new User
            {
                UserId = userId,
                IsDeleted = false,
                Balance = 100
            };

            var menuItem = new MenuItem
            {
                MenuItemId = menuItemId,
                RestaurantId = restaurantId,
                Price = 200,
                QuantityAvailable=10
            };

            var restaurant = new Restaurant
            {
                RestaurantId = restaurantId,
                MenuItems = new List<MenuItem> { menuItem }
            };

            var request = new OrderRequestDto
            {
                RestaurantId = restaurantId,
                AddressId = addressId,
                OrderedItems = new List<OrderItemDto>
                {
                    new OrderItemDto
                    {
                        MenuItemId = menuItemId,
                        Quantity = 1
                    }
                }
            };

            var address = new UserAddress
            {
                UserAddressId = addressId,
                UserId = userId
            };

            _userRepository.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _restaurantRepository.Setup(x => x.GetRestaurantByIdAsync(restaurantId)).ReturnsAsync(restaurant);
            _restaurantRepository.Setup(x => x.GetMenuItemForUpdateAsync(menuItemId)).ReturnsAsync(menuItem);
            _addressRepository.Setup(x => x.GetAddressByIdAsync(addressId, userId)).ReturnsAsync(address);

            Func<Task> Action = async () => await _orderService.ExecutePlaceOrderAsync(userId, request);
            var exception = Assert.CatchAsync<Exception>(Action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.InsufficientBalance));
            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }
    }
}
