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
    public class GetFilteredOrdersTests
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
        /// GetFilteredOrders function - Valid Request - Calls Repository
        /// </summary>
        [Test]
        public async Task GetFilteredOrders_ValidRequest_CallsRepository()
        {
            var userId = Guid.NewGuid();
            var filterOptions = new FilterOptionsDto();

            var order = OrderTestUtil.CreateMockOrder(Guid.NewGuid(), Guid.NewGuid(), userId);

            _orderRepository
                .Setup(x => x.FilterOrderAsync(userId, filterOptions))
                .ReturnsAsync(new List<Order>{order});

            await _orderService.GetFilteredOrders(userId, filterOptions);

            _orderRepository.Verify(x => x.FilterOrderAsync(userId, filterOptions),Times.Once);
        }

        /// <summary>
        /// GetFilteredOrders function - No orders from repository - Throws Exception
        /// </summary>
        [Test]
        public void GetFilteredOrders_NoOrders_ThrowsException()
        {
            var userId = Guid.NewGuid();
            var filterOptions = new FilterOptionsDto();

            _orderRepository
                .Setup(x => x.FilterOrderAsync(userId, filterOptions))
                .ReturnsAsync(new List<Order>());

            Func<Task> action = async () => await _orderService.GetFilteredOrders(userId, filterOptions);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message,Is.EqualTo(ExceptionMessages.NoOrdersToShow));

            _orderRepository.Verify(x => x.FilterOrderAsync(userId, filterOptions), Times.Once);
        }

        /// <summary>
        /// GetFilteredOrders function - Multiple orders from repositorty - Returns all orders
        /// </summary>
        [Test]
        public async Task GetFilteredOrders_MultipleOrders_ReturnsAllOrders()
        {
            var userId = Guid.NewGuid();
            var filterOptions = new FilterOptionsDto();

            var order1 = OrderTestUtil.CreateMockOrder(Guid.NewGuid(), Guid.NewGuid(), userId);
            var order2 = OrderTestUtil.CreateMockOrder(Guid.NewGuid(), Guid.NewGuid(), userId);
            var order3 = OrderTestUtil.CreateMockOrder(Guid.NewGuid(), Guid.NewGuid(), userId);

            _orderRepository
                .Setup(x => x.FilterOrderAsync(userId, filterOptions))
                .ReturnsAsync(new List<Order> { order1, order2, order3 });

            var result = await _orderService.GetFilteredOrders(userId, filterOptions);

            Assert.That(result.Count, Is.EqualTo(3));

            _orderRepository.Verify(x => x.FilterOrderAsync(userId, filterOptions), Times.Once);
        }
    }
}
