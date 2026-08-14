using DotNet_Assignment.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Assignment.Repository.Restaurants
{
    public interface IRestaurantRepository
    {
        Task<List<Restaurant>> GetRestaurantsAsync(int page, int pageSize);

        Task<Restaurant> GetRestaurantByIdAsync(Guid restaurantId);

        Task<MenuItem> GetMenuItemByIdAsync(Guid menuItemId);

        Task<MenuItem> GetMenuItemForUpdateAsync(Guid menuItemId);

        void AddRestaurant(Restaurant restaurant);

        void AddRestaurantOwner(RestaurantOwner restaurantOwner);

    }
}
