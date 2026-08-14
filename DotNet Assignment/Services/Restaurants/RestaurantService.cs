using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.Restaurants;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Enums;
using DotNet_Assignment.Constants;
using DotNet_Assignment.Data;
using DotNet_Assignment.Repository.Users;

namespace DotNet_Assignment.Services.Restaurants
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _appDbContext;

        public RestaurantService(
            IRestaurantRepository restaurantRepository,
            IUserRepository userRepository,
            AppDbContext appDbContext
            )
        {
            _restaurantRepository = restaurantRepository;
            _userRepository = userRepository;
            _appDbContext = appDbContext;
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
            var restaurants = await _restaurantRepository.GetRestaurantsAsync(page, pageSize);

            if (!restaurants.Any())
            {
                throw new Exception(ExceptionMessages.NoRestaurants);
            }

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
            var restaurant = await _restaurantRepository.GetRestaurantByIdAsync(restaurantId);

            if (restaurant == null)
            {
                throw new Exception(ExceptionMessages.RestaurantNotFound);
            }

            if (!restaurant.MenuItems.Any())
            {
                throw new Exception(ExceptionMessages.NoMenuItems);
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
            }).OrderBy(mi => mi.Rating)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        }

        /// <summary>
        /// Adds restaurant
        /// </summary>
        /// <param name="addRestaurantDto"></param>
        public async Task AddRestaurantAsync(AddRestaurantDto addRestaurantDto)
        {
            var restaurant = new Restaurant
            {
                Name = addRestaurantDto.Name,
                Description = addRestaurantDto.Description,
                Street = addRestaurantDto.Street,
                Landmark = addRestaurantDto.Landmark,
                City = addRestaurantDto.City,
                State = addRestaurantDto.State,
                Pincode = addRestaurantDto.Pincode,
                Rating = 0
            };

            _restaurantRepository.AddRestaurant(restaurant);

            await _appDbContext.SaveChangesAsync();
        }

        public async Task AddRestaurantOwner(RestaurantOwnerRequestDto restaurantOwnerRequestDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(restaurantOwnerRequestDto.UserEmail);

            if (user == null)
            {
                throw new Exception(ExceptionMessages.UserNotFound);
            }

            if (user.IsDeleted)
            {
                throw new Exception(ExceptionMessages.UserDeactivated);
            }

            var restaurant = await _restaurantRepository.GetRestaurantByIdAsync(restaurantOwnerRequestDto.RestaurantId);

            if (restaurant == null)
            {
                throw new Exception(ExceptionMessages.RestaurantNotFound);
            }

            if (!restaurant.MenuItems.Any())
            {
                throw new Exception(ExceptionMessages.NoMenuItems);
            }

            var restaurantOwner = new RestaurantOwner
            {
                RestaurantId = restaurantOwnerRequestDto.RestaurantId,
                UserId = user.UserId
            };

            if (user.Role == UserRoles.User)
            {
                user.Role = UserRoles.Owner;
            }

            _restaurantRepository.AddRestaurantOwner(restaurantOwner);

            await _appDbContext.SaveChangesAsync();
            
        }
    }
}
