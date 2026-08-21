using DotNet_Assignment.Data;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Models.Enums;
using DotNet_Assignment.Repository.Reports;
using DotNet_Assignment.Tests.Utils;
using DotNet_Assignment.Tests.Utils.RepositoryHelpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DotNet_Assignment.Tests.Repository
{
    public class ReportRepositoryTests
    {
        /// <summary>
        /// GetTop10OrderedItemsAsync - Valid data - Returns top ordered items
        /// </summary>
        [Test]
        public async Task GetTop10OrderedItemsAsync_ReturnsTopOrderedItems()
        {
            var ownerId = Guid.NewGuid();

            var data = ReportTestsUtil.CreateOrderedItems(
                ownerId, OrderStatus.Placed,
                ("Dish1", "Veg", 10),
                ("Dish2", "NonVeg", 5)
            ).AsQueryable();

            var mockContext = BuildMockContext(data);

            var repository = new ReportRepository(mockContext.Object);

            var excludedItems = new ExcludedItemsDto
            {
                ExcludeItems = new List<string>()
            };

            var result = await repository.GetTop10OrderedItemsAsync(ownerId, null, excludedItems);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));

            Assert.That(result[0].MenuItemName, Is.EqualTo("Dish1"));
            Assert.That(result[0].TotalQuantity, Is.EqualTo(10));

            Assert.That(result[1].MenuItemName, Is.EqualTo("Dish2"));
            Assert.That(result[1].TotalQuantity, Is.EqualTo(5));
        }

        /// <summary>
        /// GetTop10OrderedItemsAsync - Cancelled order - Excludes order
        /// </summary>
        [Test]
        public async Task GetTop10OrderedItemsAsync_CancelledOrder_ReturnsEmpty()
        {
            var ownerId = Guid.NewGuid();

            var data = ReportTestsUtil.CreateOrderedItems(
                ownerId,
                OrderStatus.Cancelled,
                ("Dish1", "Veg", 10)
            ).AsQueryable();

            var mockContext = BuildMockContext(data);

            var repository = new ReportRepository(mockContext.Object);

            var excludedItems = new ExcludedItemsDto
            {
                ExcludeItems = new List<string>()
            };

            var result = await repository.GetTop10OrderedItemsAsync( ownerId, null, excludedItems);

            Assert.That(result, Is.Empty);
        }

        /// <summary>
        /// GetTop10OrderedItemsAsync - Category excluded - Excludes category
        /// </summary>
        [Test]
        public async Task GetTop10OrderedItemsAsync_ExcludedCategory_ExcludesItems()
        {
            var ownerId = Guid.NewGuid();

            var data = ReportTestsUtil.CreateOrderedItems(
                ownerId,OrderStatus.Placed,
                ("Dish1", "veg", 10),
                ("Dish2", "nonveg", 5)
            ).AsQueryable();

            var mockContext = BuildMockContext(data);

            var repository = new ReportRepository(mockContext.Object);

            var excludedItems = new ExcludedItemsDto
            {
                ExcludeItems = new List<string>()
            };

            var result = await repository.GetTop10OrderedItemsAsync(ownerId, "nonveg", excludedItems);

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].MenuItemName, Is.EqualTo("Dish1"));
            Assert.That(result[0].TotalQuantity, Is.EqualTo(10));
        }

        /// <summary>
        /// GetTop10OrderedItemsAsync - Excluded item - Excludes item
        /// </summary>
        [Test]
        public async Task GetTop10OrderedItemsAsync_ExcludedItem_ExcludesItem()
        {
            var ownerId = Guid.NewGuid();

            var data = ReportTestsUtil.CreateOrderedItems(
                ownerId, OrderStatus.Placed,
                ("Dish1", "Veg", 10),
                ("Dish2", "NonVeg", 5)
            ).AsQueryable();

            var mockContext = BuildMockContext(data);

            var repository = new ReportRepository(mockContext.Object);

            var excludedItems = new ExcludedItemsDto
            {
                ExcludeItems = new List<string>
                {
                    "Dish1"
                }
            };

            var result = await repository.GetTop10OrderedItemsAsync(ownerId, null, excludedItems);

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].MenuItemName, Is.EqualTo("Dish2"));
            Assert.That(result[0].TotalQuantity, Is.EqualTo(5));
        }

        /// <summary>
        /// GetTop10OrderedItemsAsync - More than 10 items - Returns only 10
        /// </summary>
        [Test]
        public async Task GetTop10OrderedItemsAsync_MoreThanTenItems_ReturnsOnlyTen()
        {
            var ownerId = Guid.NewGuid();

            var items = Enumerable.Range(1, 11)
                .Select(i => new
                {
                    Name = "Dish" + i,
                    Category = i % 2 == 0 ? "Veg" : "NonVeg",
                    Quantity = i
                })
                .ToList();

            var data = ReportTestsUtil.CreateOrderedItems(
                ownerId, 
                OrderStatus.Placed, 
                items.Select(x => ( x.Name, x.Category,
                    x.Quantity
                )).ToArray()
            ).AsQueryable();

            var mockContext = BuildMockContext(data);

            var repository = new ReportRepository(mockContext.Object);

            var excludedItems = new ExcludedItemsDto
            {
                ExcludeItems = new List<string>()
            };

            var result = await repository.GetTop10OrderedItemsAsync(ownerId, null, excludedItems);

            Assert.That(result, Has.Count.EqualTo(10));
        }

        /// <summary>
        /// GetFrequentlyBoughtTogetherAsync - Two items in same order - Returns pair
        /// </summary>
        [Test]
        public async Task GetFrequentlyBoughtTogetherAsync_TwoItemsInSameOrder_ReturnsPair()
        {
            var item1 = ReportTestsUtil.CreateMenuItem( "Dish1", "Veg");

            var item2 = ReportTestsUtil.CreateMenuItem("Dish2", "NonVeg");

            var orderId = Guid.NewGuid();

            var data = new List<OrderedItem>
            {
                new OrderedItem
                {
                    OrderId = orderId,
                    MenuItemId = item1.MenuItemId,
                    MenuItem = item1
                },

                new OrderedItem
                {
                    OrderId = orderId,
                    MenuItemId = item2.MenuItemId,
                    MenuItem = item2
                }
            }.AsQueryable();

            var menuItems = new List<MenuItem>
            {
                item1,
                item2
            }.AsQueryable();

            var mockContext = BuildMockContext( data, menuItems);

            var repository = new ReportRepository(mockContext.Object);

            var result = await repository.GetFrequentlyBoughtTogetherAsync( Guid.NewGuid(), 10);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));

            Assert.That(result[0].Item1, Is.EqualTo("Dish2"));
            Assert.That(result[0].TotalTimesBought, Is.EqualTo(1));
        }

        /// <summary>
        /// GetFrequentlyBoughtTogetherAsync - Different orders - Does not return pair
        /// </summary>
        [Test]
        public async Task GetFrequentlyBoughtTogetherAsync_DifferentOrders_ReturnsEmpty()
        {
            var item1 = ReportTestsUtil.CreateMenuItem(
                "Dish1",
                "Veg");

            var item2 = ReportTestsUtil.CreateMenuItem(
                "Dish2",
                "NonVeg");

            var data = new List<OrderedItem>
            {
                new OrderedItem
                {
                    OrderId = Guid.NewGuid(),
                    MenuItemId = item1.MenuItemId,
                    MenuItem = item1
                },

                new OrderedItem
                {
                    OrderId = Guid.NewGuid(),
                    MenuItemId = item2.MenuItemId,
                    MenuItem = item2
                }
            }.AsQueryable();

            var menuItems = new List<MenuItem>
            {
                item1,
                item2
            }.AsQueryable();

            var mockContext = BuildMockContext(
                data,
                menuItems);

            var repository = new ReportRepository(mockContext.Object);

            var result = await repository.GetFrequentlyBoughtTogetherAsync(Guid.NewGuid(), 10);

            Assert.That(result, Is.Empty);
        }

        /// <summary>
        /// GetFrequentlyBoughtTogetherAsync - Size limits result
        /// </summary>
        [Test]
        public async Task GetFrequentlyBoughtTogetherAsync_SizeLimitsResults()
        {
            var item1 = ReportTestsUtil.CreateMenuItem("Dish1", "Veg");
            var item2 = ReportTestsUtil.CreateMenuItem("Dish2", "NonVeg");
            var item3 = ReportTestsUtil.CreateMenuItem("Dish3", "Veg");

            var orderId = Guid.NewGuid();

            var data = new List<OrderedItem>
            {
                new OrderedItem
                {
                    OrderId = orderId,
                    MenuItemId = item1.MenuItemId,
                    MenuItem = item1
                },

                new OrderedItem
                {
                    OrderId = orderId,
                    MenuItemId = item2.MenuItemId,
                    MenuItem = item2
                },

                new OrderedItem
                {
                    OrderId = orderId,
                    MenuItemId = item3.MenuItemId,
                    MenuItem = item3
                }
            }.AsQueryable();

            var menuItems = new List<MenuItem>
            {
                item1,
                item2,
                item3
            }.AsQueryable();

            var mockContext = BuildMockContext(
                data,
                menuItems);

            var repository = new ReportRepository(mockContext.Object);

            var result = await repository.GetFrequentlyBoughtTogetherAsync(Guid.NewGuid(), 2);

            Assert.That(result, Has.Count.EqualTo(2));
        }

        private Mock<AppDbContext> BuildMockContext(
            IQueryable<OrderedItem> orderedItems,
            IQueryable<MenuItem> menuItems = null)
        {
            var orderedItemSet = new Mock<DbSet<OrderedItem>>();

            orderedItemSet.As<IDbAsyncEnumerable<OrderedItem>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<OrderedItem>(
                    orderedItems.GetEnumerator()));

            orderedItemSet.As<IQueryable<OrderedItem>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<OrderedItem>(
                    orderedItems.Provider));

            orderedItemSet.As<IQueryable<OrderedItem>>()
                .Setup(m => m.Expression)
                .Returns(orderedItems.Expression);

            orderedItemSet.As<IQueryable<OrderedItem>>()
                .Setup(m => m.ElementType)
                .Returns(orderedItems.ElementType);

            orderedItemSet.As<IQueryable<OrderedItem>>()
                .Setup(m => m.GetEnumerator())
                .Returns(() => orderedItems.GetEnumerator());

            var mockContext = new Mock<AppDbContext>();

            mockContext
                .Setup(c => c.OrderedItems)
                .Returns(orderedItemSet.Object);

            if (menuItems != null)
            {
                var menuItemSet = new Mock<DbSet<MenuItem>>();

                menuItemSet.As<IDbAsyncEnumerable<MenuItem>>()
                    .Setup(m => m.GetAsyncEnumerator())
                    .Returns(new TestDbAsyncEnumerator<MenuItem>(
                        menuItems.GetEnumerator()));

                menuItemSet.As<IQueryable<MenuItem>>()
                    .Setup(m => m.Provider)
                    .Returns(new TestDbAsyncQueryProvider<MenuItem>(
                        menuItems.Provider));

                menuItemSet.As<IQueryable<MenuItem>>()
                    .Setup(m => m.Expression)
                    .Returns(menuItems.Expression);

                menuItemSet.As<IQueryable<MenuItem>>()
                    .Setup(m => m.ElementType)
                    .Returns(menuItems.ElementType);

                menuItemSet.As<IQueryable<MenuItem>>()
                    .Setup(m => m.GetEnumerator())
                    .Returns(() => menuItems.GetEnumerator());

                mockContext
                    .Setup(c => c.MenuItems)
                    .Returns(menuItemSet.Object);
            }

            return mockContext;
        }
    }
}
