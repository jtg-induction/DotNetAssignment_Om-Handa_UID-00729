using DotNet_Assignment.Constants;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Services.Orders;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace DotNet_Assignment.Controllers
{
    [RoutePrefix("api/order")]
    public class OrderController :ApiController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Places Order 
        /// </summary>
        /// <param name="orderRequestDto">Order details including restaurant id, address and items</param>
        /// <returns>Https status code with order response</returns>
        [Authorize]
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> PlaceOrderAsync(OrderRequestDto orderRequestDto)
        {
            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);

            var orderResponse = await _orderService.PlaceOrderAsync(userId, orderRequestDto);

            var response = new ApiResponseDto<OrderResponseDto>
            {
                IsSuccess= true,
                Message= SuccessMessages.OrderPlacedSuccessfully,
                Data= orderResponse
            };

            return Ok(response);
        }

        /// <summary>
        /// Gets order details of a specific order using order id
        /// </summary>
        /// <param name="orderId">Order id</param>
        /// <returns>Http status code with order details</returns>
        [Authorize]
        [HttpGet]
        [Route("{orderId}")]
        public async Task<IHttpActionResult> GetOrderDetailsAsync(Guid orderId)
        {
            var orderResponse = await _orderService.GetOrderDetailsAsync(orderId);

            var response = new ApiResponseDto<OrderDetailsResponseDto>
            {
                IsSuccess = true,
                Message = SuccessMessages.OrderDetailsFetchedSuccessfully,
                Data = orderResponse
            };

            return Ok(response);
        }

        /// <summary>
        /// Cancels a specific order
        /// </summary>
        /// <param name="orderId">Order id to cancel</param>
        /// <returns>Http status code with success message</returns>
        [Authorize]
        [HttpPost]
        [Route("cancel/{orderId}")]
        public async Task<IHttpActionResult> CancelOrderASync(Guid orderId)
        {
            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);

            await _orderService.CancelOrderAsync(orderId, userId);

            var response = new ApiResponseDto<OrderDetailsResponseDto>
            {
                IsSuccess = true,
                Message = SuccessMessages.OrderCancelledSuccessfully,
            };

            return Ok(response);
        }

    }
}
