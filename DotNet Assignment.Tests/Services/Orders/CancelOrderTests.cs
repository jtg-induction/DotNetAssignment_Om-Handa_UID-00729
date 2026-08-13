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
using System.Threading.Tasks;

namespace DotNet_Assignment.Tests.Services.Orders
{
    [TestFixture]
    public class CancelOrderTests
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
        /// CancelOrder function - Order Not Found - Throws Exception
        /// </summary>
        [Test]
        public void CancelOrder_OrderNotFound_ThrowsException()
        {
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _orderRepository.Setup(x => x.GetOrderByIdAsync(orderId)).ReturnsAsync((Order)null);

            Func<Task> Action = async () => await _orderService.CancelOrderAsync(orderId, userId);
            var exception = Assert.CatchAsync<Exception>(Action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.OrderNotFound));

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }


        /// <summary>
        /// CancelOrder function - Order Already Cancelled - Throws Exception
        /// </summary>
        [Test]
        public void CancelOrder_OrderAlreadyCancelled_ThrowsException()
        {
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var order = OrderTestUtil.CreateMockOrder(orderId, Guid.NewGuid());

            order.Status = OrderStatus.Cancelled;

            _orderRepository
                .Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync(order);

            Func<Task> Action = async () => await _orderService.CancelOrderAsync(orderId, userId);

            var exception = Assert.CatchAsync<Exception>(Action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.OrderAlreadyCancelled));

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        /// <summary>
        /// CancelOrder function - Valid request - Changes status to cancelled
        /// </summary>
        [Test]
        public async Task CancelOrder_ValidOrder_ChangesStatus()
        {
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var order = OrderTestUtil.CreateMockOrder(orderId, Guid.NewGuid());

            _orderRepository.Setup(x => x.GetOrderByIdAsync(orderId)).ReturnsAsync(order);

            await _orderService.CancelOrderAsync(orderId, userId);

            Assert.That(order.Status, Is.EqualTo(OrderStatus.Cancelled));

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);

        }

        /// <summary>
        /// CancelOrder function - Order Found - Saves Changes
        /// </summary>
        [Test]
        public async Task CancelOrder_ValidOrder_SavesChanges()
        {
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var order = OrderTestUtil.CreateMockOrder(orderId, Guid.NewGuid());

            _orderRepository.Setup(x => x.GetOrderByIdAsync(orderId)).ReturnsAsync(order);

            await _orderService.CancelOrderAsync(orderId, userId);

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
    }
}
