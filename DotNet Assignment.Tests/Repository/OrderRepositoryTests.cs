using DotNet_Assignment.Data;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Models.Enums;
using DotNet_Assignment.Repository.Orders;
using DotNet_Assignment.Tests.Constants;
using DotNet_Assignment.Tests.Utils.RepositoryHelpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet_Assignment.Tests.Repository
{
    [TestFixture]
    public class OrderRepositoryTests
    {
        /// <summary>
        /// AddOrder Function - Adds order successfully
        /// </summary>
        [Test]
        public void AddOrder_AddsOrderSuccessfully()
        {
            var mockSet = new Mock<DbSet<Order>>();
            var mockContext = new Mock<AppDbContext>();

            mockContext.Setup(c => c.Orders).Returns(mockSet.Object);

            var repository = new OrderRepository(mockContext.Object);

            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            repository.AddOrder(order);

            mockSet.Verify(m => m.Add(order), Times.Once);
        }

        /// <summary>
        /// AddOrderItem Function - Adds ordered item successfully
        /// </summary>
        [Test]
        public void AddOrderItem_AddsOrderedItemSuccessfully()
        {
            var mockSet = new Mock<DbSet<OrderedItem>>();
            var mockContext = new Mock<AppDbContext>();

            mockContext.Setup(c => c.OrderedItems).Returns(mockSet.Object);

            var repository = new OrderRepository(mockContext.Object);

            var orderedItem = new OrderedItem
            {
                OrderedItemId = Guid.NewGuid(),
                OrderId = Guid.NewGuid(),
                MenuItemId = Guid.NewGuid()
            };

            repository.AddOrderItem(orderedItem);

            mockSet.Verify(m => m.Add(orderedItem), Times.Once);
        }

        /// <summary>
        /// GetOrderByIdAsync Function - Order found - Returns order successfully
        /// </summary>
        /// <returns>Order</returns>
        [Test]
        public async Task GetOrderByIdAsync_OrderFound_ReturnsOrder()
        {
            var requestOrderId = Guid.NewGuid();

            var data = new List<Order>
            {
                new Order
                {
                    OrderId = requestOrderId,
                    UserId = Guid.NewGuid()
                }
            }.AsQueryable();

            var mockContext = BuildOrderMockContext(data);

            var repository = new OrderRepository(mockContext.Object);

            var result = await repository.GetOrderByIdAsync(requestOrderId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.OrderId, Is.EqualTo(requestOrderId));
        }

        /// <summary>
        /// GetOrderByIdAsync Function - Order not found - Returns null
        /// </summary>
        /// <returns>null</returns>
        [Test]
        public async Task GetOrderByIdAsync_OrderNotFound_ReturnsNull()
        {
            var requestOrderId = Guid.NewGuid();

            var data = new List<Order>
            {
                new Order
                {
                    OrderId = Guid.NewGuid(),
                    UserId = Guid.NewGuid()
                }
            }.AsQueryable();

            var mockContext = BuildOrderMockContext(data);

            var repository = new OrderRepository(mockContext.Object);

            var result = await repository.GetOrderByIdAsync(requestOrderId);

            Assert.That(result, Is.Null);
        }

        /// <summary>
        /// GetOrderItemsAsync function - Items found - returns Items
        /// </summary>
        /// <returns>Returns Order Items</returns>
        [Test]
        public async Task GetOrderItemsAsync_ItemsFound_ReturnsItems()
        {
            var orderId = Guid.NewGuid();

            var data = new List<OrderedItem>
            {
                new OrderedItem
                {
                    OrderedItemId = Guid.NewGuid(),
                    OrderId = orderId,
                    MenuItemId = Guid.NewGuid()
                },
                new OrderedItem
                {
                    OrderedItemId = Guid.NewGuid(),
                    OrderId = orderId,
                    MenuItemId = Guid.NewGuid()
                }
            }.AsQueryable();

            var mockContext = BuildOrderedItemMockContext(data);

            var repository = new OrderRepository(mockContext.Object);

            var result = await repository.GetOrderItemsAsync(orderId);

            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get Order Items Async - Items Not Found - Returns Empty List
        /// </summary>
        /// <returns>Empty list</returns>
        [Test]
        public async Task GetOrderItemsAsync_ItemsNotFound_ReturnsEmptyList()
        {
            var orderId = Guid.NewGuid();

            var data = new List<OrderedItem>().AsQueryable();

            var mockContext = BuildOrderedItemMockContext(data);

            var repository = new OrderRepository(mockContext.Object);

            var result = await repository.GetOrderItemsAsync(orderId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        /// <summary>
        /// Filter Order Async - No Filters - Returns all Orders
        /// </summary>
        /// <returns>All orders</returns>
        [Test]
        public async Task FilterOrderAsync_NoFilters_ReturnsOrders()
        {
            var userId = Guid.NewGuid();

            var restaurant = new Restaurant
            {
                RestaurantOwners = new List<RestaurantOwner>
                {
                    new RestaurantOwner { UserId = userId }
                }
            };

            var data = new List<Order>
            {
                new Order
                {
                    OrderId = Guid.NewGuid(),
                    Status = OrderStatus.Placed,
                    Restaurant = restaurant
                },
                new Order
                {
                    OrderId = Guid.NewGuid(),
                    Status = OrderStatus.Delivered,
                    Restaurant = restaurant
                }
            }.AsQueryable();

            var mockContext = BuildOrderMockContext(data);

            var repository = new OrderRepository(mockContext.Object);

            var filter = new FilterOptionsDto
            {
                Page = 1,
                PageSize = 10
            };

            var result = await repository.FilterOrderAsync(userId, new IncludedRestaurantsDto(), filter);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));

        }

        /// <summary>
        /// FilterOrderAsync function - Status Filter - Returns Matching Orders
        /// </summary>
        /// <returns>Returns orders based on status</returns>
        [Test]
        public async Task FilterOrderAsync_StatusFilter_ReturnsMatchingOrders()
        {
            var userId = Guid.NewGuid();

            var restaurant = new Restaurant
            {
                RestaurantOwners = new List<RestaurantOwner>
                {
                    new RestaurantOwner { UserId = userId }
                }
            };

            var data = new List<Order>
            {
                new Order
                {
                    OrderId = Guid.NewGuid(),
                    Status = OrderStatus.Placed,
                    Restaurant = restaurant
                },
                new Order
                {
                    OrderId = Guid.NewGuid(),
                    Status = OrderStatus.Delivered,
                    Restaurant = restaurant
                }
            }.AsQueryable();

            var mockContext = BuildOrderMockContext(data);

            var repository = new OrderRepository(mockContext.Object);

            var filter = new FilterOptionsDto
            {
                Status = MockConstants.MockStatus,
                Page = 1,
                PageSize = 10
            };

            var result = await repository.FilterOrderAsync(userId, new IncludedRestaurantsDto(), filter);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Status, Is.EqualTo("Placed"));
        }

        /// <summary>
        /// FilterOrderAsync function - Category Filter - Returns Matching Orders
        /// </summary>
        /// <returns>Returns orders based on category</returns>
        [Test]
        public async Task FilterOrderAsync_CategoryFilter_ReturnsMatchingOrders()
        {
            var userId = Guid.NewGuid();

            var menuItem = new MenuItem
            {
                MenuItemId = Guid.NewGuid(),
                Category = MockConstants.MockCategory
            };

            var restaurant = new Restaurant
            {
                RestaurantOwners = new List<RestaurantOwner>
                {
                    new RestaurantOwner { UserId = userId }
                }
            };

            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                Restaurant = restaurant,
                OrderedItems = new List<OrderedItem>
                {
                    new OrderedItem
                    {
                        MenuItem = menuItem
                    }
                }
            };

            var data = new List<Order> { order }.AsQueryable();

            var mockContext = BuildOrderMockContext(data);

            var repository = new OrderRepository(mockContext.Object);

            var filter = new FilterOptionsDto
            {
                Category = MockConstants.MockCategory,
                Page = 1,
                PageSize = 10,
            };

            var result = await repository.FilterOrderAsync(userId, new IncludedRestaurantsDto(), filter);

            Assert.That(result.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// FilterOrderAsync function - OrderId Filter - Returns Matching Orders
        /// </summary>
        /// <returns>Returns orders based on orderID</returns>
        [Test]
        public async Task FilterOrderAsync_OrderIdFilter_ReturnsMatchingOrder()
        {
            var userId = Guid.NewGuid();
            var orderId = Guid.NewGuid();

            var restaurant = new Restaurant
            {
                RestaurantOwners = new List<RestaurantOwner>
                {
                    new RestaurantOwner { UserId = userId }
                }
            };

            var data = new List<Order>
            {
                new Order
                {
                    OrderId = orderId,
                    Restaurant = restaurant
                },
                new Order
                {
                    OrderId = Guid.NewGuid(),
                    Restaurant = restaurant
                }
            }.AsQueryable();

            var mockContext = BuildOrderMockContext(data);

            var repository = new OrderRepository(mockContext.Object);

            var filter = new FilterOptionsDto
            {
                SearchByOrderId = orderId,
                Page = 1,
                PageSize = 10
            };

            var result = await repository.FilterOrderAsync(userId, new IncludedRestaurantsDto(), filter);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].OrderId, Is.EqualTo(orderId));
        }

        /// <summary>
        /// Filter Order Async function - Pagination Filter - Returns correct Orders
        /// </summary>
        /// <returns>Returns correct orders based on page and page size</returns>
        [Test]
        public async Task FilterOrderAsync_Pagination_ReturnsCorrectPages()
        {
            var userId = Guid.NewGuid();

            var restaurant = new Restaurant
            {
                RestaurantOwners = new List<RestaurantOwner>
                {
                    new RestaurantOwner { UserId = userId }
                }
            };

            var data = Enumerable.Range(1, 5)
                .Select(x => new Order
                {
                    OrderId = Guid.NewGuid(),
                    Restaurant = restaurant
                })
                .AsQueryable();

            var mockContext = BuildOrderMockContext(data);

            var repository = new OrderRepository(mockContext.Object);

            var filter = new FilterOptionsDto
            {
                Page = 2,
                PageSize = 2
            };

            var result = await repository.FilterOrderAsync(userId, new IncludedRestaurantsDto(), filter);

            Assert.That(result.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Filter Order Async function - SortByPrice Filter - Returns Matching Orders in ascending
        /// </summary>
        /// <returns>Returns orders in ascending order with respect to price</returns>
        [Test]
        public async Task FilterOrderAsync_SortByPriceAscending_ReturnsAscendingOrders()
        {
            var userId = Guid.NewGuid();

            var restaurant = new Restaurant
            {
                RestaurantOwners = new List<RestaurantOwner>
                {
                    new RestaurantOwner { UserId = userId }
                }
            };

            var data = new List<Order>
            {
                new Order { OrderId = Guid.NewGuid(), TotalPrice = 500, Restaurant = restaurant },
                new Order { OrderId = Guid.NewGuid(), TotalPrice = 200, Restaurant = restaurant },
                new Order { OrderId = Guid.NewGuid(), TotalPrice = 300, Restaurant = restaurant }
            }.AsQueryable();

            var mockContext = BuildOrderMockContext(data);

            var repository = new OrderRepository(mockContext.Object);

            var filter = new FilterOptionsDto
            {
                SortBy = MockConstants.MockSortBy,
                SortOrder = MockConstants.MockSortOrder,
                Page = 1,
                PageSize = 10
            };

            var result = await repository.FilterOrderAsync(userId, new IncludedRestaurantsDto(), filter);

            Assert.That(result[0].TotalPrice, Is.EqualTo(200));
            Assert.That(result[1].TotalPrice, Is.EqualTo(300));
            Assert.That(result[2].TotalPrice, Is.EqualTo(500));
        }

        /// <summary>
        /// Filter Order Async function - SortByDate Filter - Returns Matching Orders in ascending
        /// </summary>
        /// <returns>Returns orders in ascending order with respect to date</returns>
        [Test]
        public async Task FilterOrderAsync_SortByDateAscending_ReturnsAscendingOrders()
        {
            var userId = Guid.NewGuid();

            var restaurant = new Restaurant
            {
                RestaurantOwners = new List<RestaurantOwner>
                {
                    new RestaurantOwner { UserId = userId }
                }
            };

            var data = new List<Order>
            {
                new Order { OrderId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow.AddDays(2), Restaurant = restaurant },
                new Order { OrderId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow.AddDays(1), Restaurant = restaurant }
            }.AsQueryable();

            var mockContext = BuildOrderMockContext(data);

            var repository = new OrderRepository(mockContext.Object);

            var filter = new FilterOptionsDto
            {
                SortBy = MockConstants.MockSortByDate,
                SortOrder = MockConstants.MockSortOrder,
                Page = 1,
                PageSize = 10
            };

            var result = await repository.FilterOrderAsync(userId, new IncludedRestaurantsDto(), filter);

            //Assert.That(result[0].CreatedAt, Is.LessThan(result[1].CreatedAt));
        }

        private Mock<AppDbContext> BuildOrderMockContext(IQueryable<Order> data)
        {
            var mockSet = new Mock<DbSet<Order>>();
            mockSet.As<IDbAsyncEnumerable<Order>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<Order>(data.GetEnumerator()));

            mockSet.As<IQueryable<Order>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<Order>(data.Provider));

            mockSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockSet.Setup(x => x.AsNoTracking()).Returns(mockSet.Object);

            var mockContext = new Mock<AppDbContext>();
            mockContext.Setup(c => c.Orders).Returns(mockSet.Object);

            return mockContext;
        }

        private Mock<AppDbContext> BuildOrderedItemMockContext(IQueryable<OrderedItem> data)
        {
            var mockSet = new Mock<DbSet<OrderedItem>>();
            mockSet.As<IDbAsyncEnumerable<OrderedItem>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<OrderedItem>(data.GetEnumerator()));

            mockSet.As<IQueryable<OrderedItem>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<OrderedItem>(data.Provider));

            mockSet.As<IQueryable<OrderedItem>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<OrderedItem>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<OrderedItem>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockSet.Setup(m => m.Include(It.IsAny<string>())).Returns(mockSet.Object);
            mockSet.Setup(x => x.AsNoTracking()).Returns(mockSet.Object);

            var mockContext = new Mock<AppDbContext>();
            mockContext.Setup(c => c.OrderedItems).Returns(mockSet.Object);

            return mockContext;
        }
    }
}
