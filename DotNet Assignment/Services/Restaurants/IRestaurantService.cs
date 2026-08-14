using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Assignment.Services.Restaurants
{
    public interface IRestaurantService
    {
        Task<List<RestaurantResponseDto>> GetAllRestaurantsAsync(int page, int pageSize);

        Task<List<MenuItemResponseDto>> GetMenuItemsByRestaurantIdAsync(Guid restaurantId, int page, int pageSize);

        Task AddRestaurantAsync(AddRestaurantDto addRestaurantDto);

        Task AddRestaurantOwner(RestaurantOwnerRequestDto restaurantOwnerRequestDto);
    }
}
