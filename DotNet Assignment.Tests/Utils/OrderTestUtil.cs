using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Models.Enums;
using System;
using System.Collections.Generic;

namespace DotNet_Assignment.Tests.Utils
{
    public static class OrderTestUtil
    {
        /// <summary>
        /// Creates a mock order request DTO
        /// </summary>
        /// <returns>Order request DTO</returns>
        public static OrderRequestDto CreateMockOrderRequestDto()
        {
            return new OrderRequestDto
            {
                AddressId = Guid.NewGuid(),
                RestaurantId = Guid.NewGuid(),
            };
        }
        
        /// <summary>
        /// Creates Mock Order 
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="restaurantId"></param>
        /// <returns>Order</returns>
        public static Order CreateMockOrder(Guid orderId, Guid restaurantId, Guid userId)
        {
            var menuItem = new MenuItem
            {
                MenuItemId = Guid.NewGuid(),
                Name = "Dish",
                Description = "Menu Item",
                QuantityAvailable = 10
            };

            return new Order
            {
                OrderId = orderId,
                RestaurantId = restaurantId,
                Status = OrderStatus.Placed,
                UserId = userId,
                Restaurant = new Restaurant
                {
                    RestaurantId = restaurantId,
                    Name = "Restaurant"
                },
                TotalPrice = 1990,
                DeliveryAddress = "111, Street, City",
                OrderedItems = new List<OrderedItem> {
                    new OrderedItem
                    {
                        Quantity=10,
                        ItemPrice=199m,
                        OrderedItemId = Guid.NewGuid(),
                        OrderId = orderId,
                        MenuItemId = menuItem.MenuItemId,
                        MenuItem = menuItem
                    }
                }
            };
        }

        /// <summary>
        /// Creates A mock restaurant
        /// </summary>
        /// <param name="restaurantId"></param>
        /// <returns>restaurant</returns>
        public static Restaurant CreateMockRestaurant(Guid restaurantId)
        {
            return new Restaurant
            {
                RestaurantId = restaurantId,
                Name = "Restaurant"
            };
        }

        /// <summary>
        /// Creates a list of mock ordered items
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="menuItemId"></param>
        /// <returns>List of ordered items</returns>
        public static List<OrderedItem> CreateMockOrderedItems(Guid orderId, Guid menuItemId)
        {
            var menuItem = new MenuItem
            {
                MenuItemId = menuItemId,
                Name = "Pizza",
                Description = "Cheese Pizza"
            };

            return new List<OrderedItem>
            {
                new OrderedItem
                {
                    OrderId = orderId,
                    MenuItemId = menuItemId,
                    Quantity = 2,
                    ItemPrice = 250,
                    MenuItem = menuItem
                }
            };
        }
    }
}
