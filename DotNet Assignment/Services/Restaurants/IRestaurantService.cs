using DotNet_Assignment.Models.DTO;
using System;
using System.Threading.Tasks;

namespace DotNet_Assignment.Services.Restaurants
{
    public interface IRestaurantService
    {
        Task<PagedResponseDto<RestaurantResponseDto>> GetAllRestaurantsAsync(int page, int pageSize);

        Task<PagedResponseDto<MenuItemResponseDto>> GetMenuItemsByRestaurantIdAsync(Guid restaurantId, int page, int pageSize);

        Task AddRestaurantAsync(AddRestaurantDto addRestaurantDto);

        Task AddRestaurantOwnerAsync(RestaurantOwnerRequestDto restaurantOwnerRequestDto);
    }
}
