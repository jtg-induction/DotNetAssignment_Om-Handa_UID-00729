using DotNet_Assignment.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Assignment.Tests.Utils
{
    public static class RestaurantTestUtil
    {
        /// <summary>
        /// Creates a list of mock restaurants
        /// </summary>
        /// <param name="restaurantId"></param>
        /// <returns>List of restaurants</returns>
        public static List<Restaurant> MockRestaurantHelper(Guid restaurantId)
        {
            return new List<Restaurant>{

                new Restaurant {
                    RestaurantId = restaurantId,
                    Name = "Restaurant",
                    Description = "Description",
                    Street = "Street",
                    Landmark = "Landmark",
                    City = "City",
                    State = "State",
                    Pincode = "111111",
                    Rating = 5,
                    IsOpen = true
                }
            };
        }

        /// <summary>
        /// list of mock menu items
        /// </summary>
        /// <param name="menuItemId"></param>
        /// <returns>list of menu items</returns>
        public static List<MenuItem> MockMenuItemHelper(Guid menuItemId)
        {
            return new List<MenuItem>{

                 new MenuItem{
                    MenuItemId = menuItemId,
                    Name = "Pizza",
                    Description = "Cheese Pizza",
                    Rating = 4.5m,
                    Category = "Italian",
                    Price = 300,
                    InStock = true
                 }
            };
        }

    }
}
