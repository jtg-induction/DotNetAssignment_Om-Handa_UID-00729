using DotNet_Assignment.Data;
using DotNet_Assignment.Models.DTO;
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
        /// GetFilteredOrders function - Valid Request - Calls Repository
        /// </summary>
        [Test]
        public async Task GetFilteredOrders_ValidRequest_CallsRepository()
        {
            var userId = Guid.NewGuid();
            var filterOptions = new FilterOptionsDto();

            var order = OrderTestUtil.CreateMockOrder(Guid.NewGuid(), Guid.NewGuid(), userId, Guid.NewGuid());

            var mockDto = new OrderDetailsResponseDto
            {
                OrderId = Guid.NewGuid(),
                TotalPrice = 150.00m,
                Status = "Pending",
                DeliveryAddress = "123 Test Street",
                RestaurantName = "Test Restaurant",
                OrderedItems = new List<MenuDetailsResponseDto>()
            };

            _orderRepository
                .Setup(x => x.FilterOrderAsync(userId, filterOptions))
                .ReturnsAsync(new List<OrderDetailsResponseDto>{mockDto});

            await _orderService.GetFilteredOrders(userId, filterOptions);

            _orderRepository.Verify(x => x.FilterOrderAsync(userId, filterOptions),Times.Once);
        }

        /// <summary>
        /// GetFilteredOrders function - No orders from repository - Returns Empty List
        /// </summary>
        [Test]
        public async Task GetFilteredOrders_NoOrders_ReturnsEmptyList()
        {
            var userId = Guid.NewGuid();
            var filterOptions = new FilterOptionsDto();

            _orderRepository
                .Setup(x => x.FilterOrderAsync(userId, filterOptions))
                .ReturnsAsync(new List<OrderDetailsResponseDto> {});

            var result = await _orderService.GetFilteredOrders(userId, filterOptions);

            Assert.That(result.Count ,Is.EqualTo(0));

            _orderRepository.Verify(x => x.FilterOrderAsync(userId, filterOptions), Times.Once);
        }

        /// <summary>
        /// GetFilteredOrders function - Multiple orders from repository - Returns all orders
        /// </summary>
        [Test]
        public async Task GetFilteredOrders_MultipleOrders_ReturnsAllOrders()
        {
            var userId = Guid.NewGuid();
            var filterOptions = new FilterOptionsDto();

            var dto1 = new OrderDetailsResponseDto { OrderId = Guid.NewGuid(), TotalPrice = 50.00m, Status = "Pending", OrderedItems = new List<MenuDetailsResponseDto>() };
            var dto2 = new OrderDetailsResponseDto { OrderId = Guid.NewGuid(), TotalPrice = 75.00m, Status = "Completed", OrderedItems = new List<MenuDetailsResponseDto>() };
            var dto3 = new OrderDetailsResponseDto { OrderId = Guid.NewGuid(), TotalPrice = 120.00m, Status = "Cancelled", OrderedItems = new List<MenuDetailsResponseDto>() };

            var mockDto = new List<OrderDetailsResponseDto> { dto1, dto2, dto3 };

            _orderRepository
                .Setup(x => x.FilterOrderAsync(userId, filterOptions))
                .ReturnsAsync( mockDto );

            var result = await _orderService.GetFilteredOrders(userId, filterOptions);

            Assert.That(result.Count, Is.EqualTo(3));

            _orderRepository.Verify(x => x.FilterOrderAsync(userId, filterOptions), Times.Once);
        }
    }
}
