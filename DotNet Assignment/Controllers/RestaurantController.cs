using DotNet_Assignment.Constants;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Models.Enums;
using DotNet_Assignment.Services.Orders;
using DotNet_Assignment.Services.Restaurants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Security;

namespace DotNet_Assignment.Controllers
{
    [RoutePrefix("api/restaurant")]
    public class RestaurantController : ApiController
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        /// <summary>
        /// Gets list of all Restaurant with pagination
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of restaurants to show on page</param>
        /// <returns>Http status code with restaurants list</returns>
        [Authorize]
        [HttpGet]
        [Route("{page}/{pageSize}")]
        public async Task<IHttpActionResult> GetRestaurantsAsync(int page= 1, int pageSize= 10)
        {
            var data = await _restaurantService.GetAllRestaurantsAsync(page, pageSize);

            var response = new ApiResponseDto<List<RestaurantResponseDto>>
            {
                IsSuccess = true,
                Message = SuccessMessages.RestaurantsFetchedSuccessfully,
                Data= data
            };

            return Ok(response);
        }

        /// <summary>
        /// Return all menu items of a restaurant, with pagination
        /// </summary>
        /// <param name="restaurantId">Restaurant Id</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Size of items to be shown on the page</param>
        /// <returns>Http response Token with Menu item data</returns>
        [Authorize]
        [HttpGet]
        [Route("{restaurantId:guid}/{page}/{pageSize}")]
        public async Task<IHttpActionResult> GetMenuItemsAsync(Guid restaurantId, int page = 1, int pageSize = 10)
        {
            var data = await _restaurantService.GetMenuItemsByRestaurantIdAsync(restaurantId, page, pageSize);

            var response = new ApiResponseDto<List<MenuItemResponseDto>>
            {
                IsSuccess = true,
                Message = SuccessMessages.ItemsFetchedSuccessfully,
                Data = data
            };

            return Ok(response);
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpPost]
        [Route("add")]
        public async Task<IHttpActionResult> AddRestaurants(AddRestaurantDto addRestaurantDto)
        {
            await _restaurantService.AddRestaurantAsync(addRestaurantDto);

            var response = new ApiResponseDto<List<MenuItemResponseDto>>
            {
                IsSuccess = true,
                Message = SuccessMessages.RestaurantAddedSuccessfully
            };

            return Ok(response);
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpPost]
        [Route("owner")]
        public async Task<IHttpActionResult> AddRestaurants(RestaurantOwnerRequestDto restaurantOwnerRequestDto)
        {
            await _restaurantService.AddRestaurantOwner(restaurantOwnerRequestDto);

            var response = new ApiResponseDto<List<MenuItemResponseDto>>
            {
                IsSuccess = true,
                Message = SuccessMessages.OwnerLinkedSuccessfully
            };

            return Ok(response);
        }
    }
}
