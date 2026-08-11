using DotNet_Assignment.Constants;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Services.Orders;
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
    public class MenuItemController : ApiController
    {
        private readonly IMenuItemService _menuItemService;

        public MenuItemController(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        [Authorize]
        [HttpGet]
        [Route("{restaurantId:guid}")]
        public async Task<IHttpActionResult> GetMenuItemById(Guid restaurantId)
        {
            var data = await _menuItemService.GetMenuItemsByRestaurantIdAsync(restaurantId);

            var response = new ApiResponseDto<List<MenuItemResponseDto>>
            {
                IsSuccess = true,
                Message = SuccessMessages.ItemsFetchedSuccessfully,
                Data = data
            };

            return Ok(response);
        }
    }
}
