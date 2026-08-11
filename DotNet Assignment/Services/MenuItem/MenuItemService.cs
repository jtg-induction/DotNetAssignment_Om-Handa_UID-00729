using DotNet_Assignment.Constants;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Repository.Restaurants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DotNet_Assignment.Services.Orders
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IRestaurantRepository _restaurantRepository;

        public MenuItemService(IRestaurantRepository restaurantRepository)
        {
            _restaurantRepository = restaurantRepository;
        }

        public async Task<List<MenuItemResponseDto>> GetMenuItemsByRestaurantIdAsync(Guid restaurantId)
        {
            var restaurant = await _restaurantRepository.GetRestaurantById(restaurantId);

            if(restaurant == null)
            {
                throw new Exception(ExceptionMessages.RestaurantNotFound);
            }

            return restaurant.MenuItems.Select(mi => new MenuItemResponseDto
            {
                MenuItemId = mi.MenuItemId,
                Name = mi.Name,
                Description = mi.Description,
                Rating = mi.Rating,
                Category = mi.Category,
                Price = mi.Price,
                InStock = mi.InStock
            }).ToList();
        }
    }
}