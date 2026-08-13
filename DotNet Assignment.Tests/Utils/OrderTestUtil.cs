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
                OrderedItems = new List<OrderItemDto>{
                    new OrderItemDto{
                        Quantity= 10,
                        MenuItemId= Guid.NewGuid()
                    },
                    new OrderItemDto{
                        Quantity= 2,
                        MenuItemId= Guid.NewGuid()
                    }
                }
            };
        }

        /// <summary>
        /// Creates Mock Order 
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="restaurantId"></param>
        /// <returns>Order</returns>
        public static Order CreateMockOrder(Guid orderId, Guid restaurantId)
        {
            return new Order
            {
                OrderId = orderId,
                RestaurantId = restaurantId,
                Status = OrderStatus.Placed
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
