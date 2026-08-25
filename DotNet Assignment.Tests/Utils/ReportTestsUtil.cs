using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Assignment.Tests.Utils
{
    public static class ReportTestsUtil
    {
        public static Order CreateOrder(Guid ownerId,OrderStatus status = OrderStatus.Placed)
        {
            var restaurant = new Restaurant
            {
                RestaurantId = Guid.NewGuid(),

                RestaurantOwners = new List<RestaurantOwner>
                {
                    new RestaurantOwner
                    {
                        UserId = ownerId
                    }
                }
            };

            return new Order
            {
                OrderId = Guid.NewGuid(),
                RestaurantId = restaurant.RestaurantId,
                Restaurant = restaurant,
                Status = status,
            };
        }

        public static MenuItem CreateMenuItem(string name,string category)
        {
            return new MenuItem
            {
                MenuItemId = Guid.NewGuid(),
                Name = name,
                Category = category,

            };
        }

        public static List<OrderedItem> CreateOrderedItems(Guid ownerId, OrderStatus status = OrderStatus.Placed, params (string Name, string Category, int Quantity)[] items)
        {
            var order = CreateOrder(ownerId, status);

            return items.Select(x =>
            {
                var menuItem = new MenuItem
                {
                    MenuItemId = Guid.NewGuid(),
                    Name = x.Name,
                    Category = x.Category,
                    RestaurantId = order.RestaurantId,
                    Restaurant = order.Restaurant
                };

                return new OrderedItem
                {
                    OrderId = order.OrderId,
                    Order = order,
                    MenuItemId = menuItem.MenuItemId,
                    MenuItem = menuItem,
                    Quantity = x.Quantity
                };

            }).ToList();
        }
    }
}
