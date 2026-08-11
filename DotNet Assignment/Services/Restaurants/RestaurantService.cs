using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.Restaurants;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using DotNet_Assignment.Models.DTO;

namespace DotNet_Assignment.Services.Restaurants
{
    public class RestaurantService :IRestaurantService
    { 
        private readonly IRestaurantRepository _restaurantRepository;

        public RestaurantService(IRestaurantRepository restaurantRepository)
        {
            _restaurantRepository = restaurantRepository;
        }

        public async Task<List<RestaurantResponseDto>> GetAllRestaurantsAsync()
        {
            var restaurants = await _restaurantRepository.GetRestaurantsAsync();

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

                MenuItems = r.MenuItems.Select(mi => new MenuItemResponseDto
                {
                    MenuItemId = mi.MenuItemId,
                    Name = mi.Name,
                    Description = mi.Description,
                    Rating = mi.Rating,
                    Category = mi.Category,
                    Price = mi.Price,
                    InStock = mi.InStock
                }).ToList()
            }).ToList();

        }
    }
}