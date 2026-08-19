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
        public async Task<List<Restaurant>> GetPagedRestaurantsAsync(int page, int pageSize)
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

        /// <summary>
        /// Adds restaurant owners to owner table
        /// </summary>
        /// <param name="restaurantOwner"></param>
        public void AddRestaurantOwner(RestaurantOwner restaurantOwner)
        {
            _context.RestaurantsOwner.Add(restaurantOwner);
        }

        /// Gets paginated list of menu items using page and pageSize
        /// </summary>
        /// <param name="restaurantId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns>paginated list of menu items</returns>
        public async Task<List<MenuItem>> GetPagedMenuItems(Guid restaurantId, int page, int pageSize)
        {
            return await _context.MenuItems
                .Where(mi => mi.RestaurantId == restaurantId && !mi.IsDeleted)
                .OrderBy(mi => mi.Rating)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Checks is user exists or not
        /// </summary>
        /// <param name="restaurantId"></param>
        /// <returns>true if restaurant exists else false</returns>
        public async Task<bool> RestaurantExists(Guid restaurantId)
        {
            return await _context.Restaurants.AnyAsync(x => x.RestaurantId == restaurantId);
        }
    }
}
