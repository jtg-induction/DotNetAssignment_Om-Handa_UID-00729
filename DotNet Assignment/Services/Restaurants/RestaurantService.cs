using DotNet_Assignment.Constants;
using DotNet_Assignment.Data;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.Restaurants;
using DotNet_Assignment.Repository.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DotNet_Assignment.Services.Restaurants
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepository _restaurantRepository;

        public RestaurantService(IRestaurantRepository restaurantRepository)
        {
            _restaurantRepository = restaurantRepository;
        }

        /// <summary>
        /// Gets all restaurants with pagination
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Restaurants to be shown on one page</param>
        /// <returns>List of restaurants</returns>
        /// <exception cref="Exception">If no restaurants found</exception>
        public async Task<List<RestaurantResponseDto>> GetAllRestaurantsAsync(int page, int pageSize)
        {
            var restaurants = await _restaurantRepository.GetPagedRestaurantsAsync(page, pageSize);

            return restaurants.Select(r => new RestaurantResponseDto
            {
                RestaurantId = r.RestaurantId,
                Name = r.Name,
                Description = r.Description,
                Street = r.Street,
                Landmark = r.Landmark,
                City = r.City,
                State = r.State,
                Pincode = r.Pincode,
                Rating = r.Rating,
                IsOpen = r.IsOpen,
            }).ToList();
        }

        /// <summary>
        /// Gets all menu items of an restaurant with pagination
        /// </summary>
        /// <param name="restaurantId">Restaurant ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Items to be shown on a page</param>
        /// <returns>List of menu items</returns>
        /// <exception cref="Exception">If restaurant not found, no items in restaurant</exception>
        public async Task<List<MenuItemResponseDto>> GetMenuItemsByRestaurantIdAsync(Guid restaurantId, int page, int pageSize)
        {

            if (!await _restaurantRepository.RestaurantExists(restaurantId))
            {
                throw new KeyNotFoundException(ExceptionMessages.RestaurantNotFound);
            }

            var menuItems = await _restaurantRepository.GetPagedMenuItems(restaurantId, page, pageSize);

            return menuItems.Select(mi => new MenuItemResponseDto
            {
                MenuItemId = mi.MenuItemId,
                Name = mi.Name,
                Description = mi.Description,
                Rating = mi.Rating,
                Category = mi.Category,
                Price = mi.Price,
            }).ToList();
        }

    }
}
