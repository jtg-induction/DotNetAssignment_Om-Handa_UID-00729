using DotNet_Assignment.Data;
using DotNet_Assignment.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DotNet_Assignment.Repository.Restaurants
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly AppDbContext _context;

        public RestaurantRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets restairant by restaurant id with pagination
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Restaurants to show on one page</param>
        /// <returns></returns>
        public async Task<List<Restaurant>> GetRestaurantsAsync(int page, int pageSize)
        {
            return await _context.Restaurants
                .OrderBy(r => r.Rating)
                .Skip((page-1)*pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a restaurant using restaurant Id
        /// </summary>
        /// <param name="restaurantId"></param>
        /// <returns>Restaurant</returns>
        public async Task<Restaurant> GetRestaurantByIdAsync(Guid restaurantId)
        {
            return await _context.Restaurants.SingleOrDefaultAsync(r => r.RestaurantId == restaurantId);
        }
        
        /// <summary>
        /// Gets menu item by item id
        /// </summary>
        /// <param name="menuItemId"></param>
        /// <returns>Menu Item</returns>
        public async Task<MenuItem> GetMenuItemByIdAsync(Guid menuItemId)
        {
            return await _context.MenuItems.SingleOrDefaultAsync(mi => mi.MenuItemId == menuItemId);
        }

        /// <summary>
        /// Applies update and row lock and returns menuItem
        /// </summary>
        /// <param name="menuItemId"></param>
        /// <returns>Menu item</returns>
        public async Task<MenuItem> GetMenuItemForUpdateAsync(Guid menuItemId)
        {
            return await _context.MenuItems.SqlQuery("SELECT * FROM  MenuItems WITH (UPDLOCK, ROWLOCK) where MenuItemId = @p0", menuItemId).SingleOrDefaultAsync();
        }

        /// <summary>
        /// Adds restaurant to DB context
        /// </summary>
        /// <param name="restaurant"></param>
        public void AddRestaurant(Restaurant restaurant)
        {
            _context.Restaurants.Add(restaurant);
        }

        public void AddRestaurantOwner(RestaurantOwner restaurantOwner)
        {
            _context.RestaurantsOwner.Add(restaurantOwner);
        }
    }
}
