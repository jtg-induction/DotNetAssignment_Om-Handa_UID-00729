using DotNet_Assignment.Constants;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Services.Restaurants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

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

        [Authorize]
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetRestaurantsAsync()
        {
            var data = await _restaurantService.GetAllRestaurantsAsync();

            var response = new ApiResponseDto<List<RestaurantResponseDto>>
            {
                IsSuccess = true,
                Message = SuccessMessages.RestaurantsFetchedSuccessfully,
                Data= data
            };

            return Ok(response);
        }
    }
}
