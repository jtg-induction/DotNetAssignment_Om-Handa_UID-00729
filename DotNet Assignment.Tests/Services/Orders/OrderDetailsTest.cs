using DotNet_Assignment.Constants;
using DotNet_Assignment.Data;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Models.Enums;
using DotNet_Assignment.Repository.Address;
using DotNet_Assignment.Repository.Orders;
using DotNet_Assignment.Repository.Restaurants;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.Orders;
using DotNet_Assignment.Tests.Utils;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Assignment.Tests.Services.Orders
{
    [TestFixture]
    public class OrderDetailsTest
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
        /// GetOrderDetails Function - Order Found - Returns Details
        /// </summary>
        [Test]
        public async Task GetOrderDetails_OrderFound_ReturnsOrderDetails()
        {
            var orderId = Guid.NewGuid();
            var restaurantId = Guid.NewGuid();
            var menuItemId = Guid.NewGuid();

            var order = OrderTestUtil.CreateMockOrder(orderId, restaurantId, Guid.NewGuid());

            var orderedItems = OrderTestUtil.CreateMockOrderedItems(orderId, menuItemId);

            var restaurant = OrderTestUtil.CreateMockRestaurant(restaurantId);

            _orderRepository.Setup(x => x.GetOrderByIdAsync(orderId)).ReturnsAsync(order);
            _orderRepository.Setup(x => x.GetOrderItemsAsync(orderId)).ReturnsAsync(orderedItems);
            _restaurantRepository.Setup(x => x.GetRestaurantByIdAsync(restaurantId)).ReturnsAsync(restaurant);

            var result = await _orderService.GetOrderDetailsAsync(orderId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.OrderId, Is.EqualTo(orderId));
            Assert.That(result.RestaurantName, Is.EqualTo("Restaurant"));
        }

        /// <summary>
        /// GetOrderDetails Function - Order Not Found - Returns Exception
        /// </summary>
        [Test]
        public void GetOrderDetails_OrderNotFound_ThrowsException()
        {
            var orderId = Guid.NewGuid();

            _orderRepository.Setup(x => x.GetOrderByIdAsync(orderId)).ReturnsAsync((Order)null);

            Func<Task> Action = async () => await _orderService.GetOrderDetailsAsync(orderId);
            var exception = Assert.CatchAsync<Exception>(Action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.OrderNotFound));
        }

        /// <summary>
        /// GetOrderDetails Function - Order Found - Returns ordered Items
        /// </summary>
        [Test]
        public async Task GetOrderDetails_OrderFound_ReturnsOrderedItems()
        {
            var orderId = Guid.NewGuid();
            var restaurantId = Guid.NewGuid();
            var menuItemId = Guid.NewGuid();

            var order = OrderTestUtil.CreateMockOrder(orderId, restaurantId, Guid.NewGuid());

            var orderedItems = OrderTestUtil.CreateMockOrderedItems(orderId, menuItemId);

            var restaurant = OrderTestUtil.CreateMockRestaurant(restaurantId);

            _orderRepository.Setup(x => x.GetOrderByIdAsync(orderId)).ReturnsAsync(order);
            _orderRepository.Setup(x => x.GetOrderItemsAsync(orderId)).ReturnsAsync(orderedItems);
            _restaurantRepository.Setup(x => x.GetRestaurantByIdAsync(restaurantId)).ReturnsAsync(restaurant);

            var result = await _orderService.GetOrderDetailsAsync(orderId);

            Assert.That(result.OrderedItems.Count, Is.EqualTo(1));
            Assert.That(result.OrderedItems[0].Name, Is.EqualTo("Pizza"));
            Assert.That(result.OrderedItems[0].Description, Is.EqualTo("Cheese Pizza"));
            Assert.That(result.OrderedItems[0].Price, Is.EqualTo(250));
            Assert.That(result.OrderedItems[0].Quantity, Is.EqualTo(2));
        }

        /// <summary>
        /// GetOrderDetails Function - Order Found - Calls Repository function
        /// </summary>
        [Test]
        public async Task GetOrderDetails_OrderFound_CallsRepositories()
        {
            var orderId = Guid.NewGuid();
            var restaurantId = Guid.NewGuid();

            var order = OrderTestUtil.CreateMockOrder(orderId, restaurantId, Guid.NewGuid());

            var restaurant = OrderTestUtil.CreateMockRestaurant(restaurantId);

            _orderRepository.Setup(x => x.GetOrderByIdAsync(orderId)).ReturnsAsync(order);
            _orderRepository.Setup(x => x.GetOrderItemsAsync(orderId)).ReturnsAsync(new List<OrderedItem>());
            _restaurantRepository.Setup(x => x.GetRestaurantByIdAsync(restaurantId)).ReturnsAsync(restaurant);

            await _orderService.GetOrderDetailsAsync(orderId);

            _orderRepository.Verify(x => x.GetOrderByIdAsync(orderId), Times.Once);
            _orderRepository.Verify(x => x.GetOrderItemsAsync(orderId), Times.Once);
            _restaurantRepository.Verify(x => x.GetRestaurantByIdAsync(restaurantId), Times.Once);
        }
    }
}
