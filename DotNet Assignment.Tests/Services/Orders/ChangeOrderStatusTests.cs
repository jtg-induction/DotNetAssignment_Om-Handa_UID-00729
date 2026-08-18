using DotNet_Assignment.Constants;
using DotNet_Assignment.Data;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Models.Enums;
using DotNet_Assignment.Repository.Address;
using DotNet_Assignment.Repository.Orders;
using DotNet_Assignment.Repository.Restaurants;
using DotNet_Assignment.Repository.Transaction;
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
    public class ChangeOrderStatusTests
    {
        private Mock<IOrderRepository> _orderRepository;
        private Mock<IRestaurantRepository> _restaurantRepository;
        private Mock<IUserRepository> _userRepository;
        private Mock<IAddressRepository> _addressRepository;
        private Mock<ITransactionRepository> _transactionRepository;
        private Mock<AppDbContext> _appDbContext;
        private Mock<ITransaction> _transaction;
        private OrderService _orderService;

        [SetUp]
        public void Setup()
        {
            _orderRepository = new Mock<IOrderRepository>();
            _restaurantRepository = new Mock<IRestaurantRepository>();
            _userRepository = new Mock<IUserRepository>();
            _addressRepository = new Mock<IAddressRepository>();
            _transactionRepository = new Mock<ITransactionRepository>();
            _appDbContext = new Mock<AppDbContext>();
            _transaction = new Mock<ITransaction>();

            _orderService = new OrderService(
                _orderRepository.Object,
                _restaurantRepository.Object,
                _addressRepository.Object,
                _userRepository.Object,
                _transactionRepository.Object,
                _appDbContext.Object);
        }

        /// <summary>
        /// ChangeOrderStatus function - Order Not Found - ThrowsException
        /// </summary>
        [Test]
        public void ChangeOrderStatus_OrderNotFound_ThrowsException()
        {
            var orderId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var request = new ChangeOrderStatusDto { Status = OrderStatus.Placed };

            _orderRepository
                .Setup(x => x.GetOrderForUpdateAsync(orderId))
                .ReturnsAsync((Order)null);

            Func<Task> action = async () => await _orderService.ChangeOrderStatusAsync(request, orderId, ownerId);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.OrderNotFound));

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        /// <summary>
        /// ChangeOrderStatus function - Unauthorized owner - ThrowsException
        /// </summary>
        [Test]
        public void ChangeOrderStatus_UnauthorizedOwner_ThrowsException()
        {
            var orderId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var request = new ChangeOrderStatusDto { Status = OrderStatus.Placed };

            var order = OrderTestUtil.CreateMockOrder(orderId, Guid.NewGuid(), Guid.NewGuid());

            _orderRepository
                .Setup(x => x.GetOrderForUpdateAsync(orderId))
                .ReturnsAsync(order);

            Func<Task> action = async () => await _orderService.ChangeOrderStatusAsync(request, orderId, ownerId);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.Unauthorized));

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        /// <summary>
        /// ChangeOrderStatus function - Same Status - ThrowsException
        /// </summary>
        [Test]
        public void ChangeOrderStatus_SameStatus_ThrowsException()
        {
            var orderId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();

            var order = OrderTestUtil.CreateMockOrder(orderId, Guid.NewGuid(), ownerId);

            order.Status = OrderStatus.Placed;

            order.Restaurant.RestaurantOwners = new List<RestaurantOwner>
            {
                new RestaurantOwner { UserId = ownerId }
            };

            var request = new ChangeOrderStatusDto
            {
                Status = OrderStatus.Placed
            };

            _orderRepository
                .Setup(x => x.GetOrderForUpdateAsync(orderId))
                .ReturnsAsync(order);

            Func<Task> action = async () => await _orderService.ChangeOrderStatusAsync(request, orderId, ownerId);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.OrderStatusAlreadyChanged));

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        /// <summary>
        /// ChangeOrderStatus function - Status moved backwards - ThrowsException
        /// </summary>
        [Test]
        public void ChangeOrderStatus_StatusMovedBackwards_ThrowsException()
        {
            var orderId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();

            var order = OrderTestUtil.CreateMockOrder(orderId, Guid.NewGuid(), ownerId);
            order.Status = OrderStatus.Delivered;

            order.Restaurant.RestaurantOwners = new List<RestaurantOwner>
            {
                new RestaurantOwner { UserId = ownerId }
            };

            var request = new ChangeOrderStatusDto
            {
                Status = OrderStatus.Placed
            };

            _orderRepository
                .Setup(x => x.GetOrderForUpdateAsync(orderId))
                .ReturnsAsync(order);

            Func<Task> action = async () => await _orderService.ChangeOrderStatusAsync(request, orderId, ownerId);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.StatusCantBeChanged));

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        /// <summary>
        /// ChangeOrderStatus function - Valid status - Changes status
        /// </summary>
        [Test]
        public async Task ChangeOrderStatus_ValidStatus_ChangesStatus()
        {
            var orderId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();

            var order = OrderTestUtil.CreateMockOrder(orderId, Guid.NewGuid(), ownerId);
            order.Status = OrderStatus.Placed;

            order.Restaurant.RestaurantOwners = new List<RestaurantOwner>
            {
                new RestaurantOwner { UserId = ownerId }
            };

            var request = new ChangeOrderStatusDto
            {
                Status = OrderStatus.Accepted
            };

            _orderRepository
                .Setup(x => x.GetOrderForUpdateAsync(orderId))
                .ReturnsAsync(order);

            await _orderService.ChangeOrderStatusAsync(request, orderId, ownerId);

            Assert.That(order.Status, Is.EqualTo(OrderStatus.Accepted));
        }

        /// <summary>
        /// ChangeOrderStatus function - Valid Changes - Saves Changes
        /// </summary>
        [Test]
        public async Task ChangeOrderStatus_ValidStatus_SavesChanges()
        {
            var orderId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();

            var order = OrderTestUtil.CreateMockOrder(orderId, Guid.NewGuid(), ownerId);

            order.Status = OrderStatus.Placed;

            order.Restaurant.RestaurantOwners = new List<RestaurantOwner>
            {
                new RestaurantOwner { UserId = ownerId }
            };

            var request = new ChangeOrderStatusDto
            {
                Status = OrderStatus.Accepted
            };

            _orderRepository
                .Setup(x => x.GetOrderForUpdateAsync(orderId))
                .ReturnsAsync(order);

            await _orderService.ChangeOrderStatusAsync(request, orderId, ownerId);

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
    }
}
