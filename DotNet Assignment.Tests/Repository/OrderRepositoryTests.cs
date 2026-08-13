using DotNet_Assignment.Data;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.Orders;
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

        public Mock<AppDbContext> BuildOrderMockContext(IQueryable<Order> data)
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

            var mockContext = new Mock<AppDbContext>();
            mockContext.Setup(c => c.Orders).Returns(mockSet.Object);

            return mockContext;
        }

        public Mock<AppDbContext> BuildOrderedItemMockContext(IQueryable<OrderedItem> data)
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

            var mockContext = new Mock<AppDbContext>();
            mockContext.Setup(c => c.OrderedItems).Returns(mockSet.Object);

            return mockContext;
        }
    }
}
