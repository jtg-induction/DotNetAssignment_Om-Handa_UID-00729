using DotNet_Assignment.Constants;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Enums;
using DotNet_Assignment.Services.Orders;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace DotNet_Assignment.Controllers
{
    [RoutePrefix("api/orders")]
    public class OrderController : ApiController
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
                IsSuccess = true,
                Message = SuccessMessages.OrderPlacedSuccessfully,
                Data = orderResponse
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
        [Route("{orderId:guid}")]
        public async Task<IHttpActionResult> GetOrderDetailsAsync(Guid orderId)
        {
            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);

            var orderResponse = await _orderService.GetOrderDetailsAsync(orderId, userId);

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
        [HttpPatch]
        [Route("cancel/{orderId:guid}")]
        public async Task<IHttpActionResult> CancelOrderAsync(Guid orderId)
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

        /// <summary>
        /// Changes Order Status (Owner)
        /// </summary>
        /// <param name="orderStatusDto">Status to be changed to</param>
        /// <param name="orderId"></param>
        /// <returns>Http response with Success message</returns>
        [Authorize(Roles = nameof(UserRoles.Owner))]
        [HttpPost]
        [Route("{orderId:guid}")]
        public async Task<IHttpActionResult> ChangeOrderStatusAsync(ChangeOrderStatusDto orderStatusDto, Guid orderId)
        {
            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);

            await _orderService.ChangeOrderStatusAsync(orderStatusDto, orderId, userId);

            var response = new ApiResponseDto<OrderDetailsResponseDto>
            {
                IsSuccess = true,
                Message = SuccessMessages.OrderStatusUpdatedSuccessfully,
            };

            return Ok(response);
        }

        /// <summary>
        /// Filters orders based on query params
        /// </summary>
        /// <param name="category"></param>
        /// <param name="status"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortOrder"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchByOrderId"></param>
        /// <returns>Http response with success message and filtered orders</returns>
        [Authorize(Roles = nameof(UserRoles.Owner))]
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> FilterOrder(
                string category = null,
                string status = null,
                string sortBy = "date",
                string sortOrder = "desc",
                int page = 1,
                int pageSize = 10,
                Guid? searchByOrderId = null
            )
        {
            var filterOptions = new FilterOptionsDto
            {
                category = category,
                status = status,
                SortBy = sortBy,
                SortOrder = sortOrder,
                Page = page,
                PageSize = pageSize,
                SearchByOrderId = searchByOrderId
            };

            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);

            var result = await _orderService.GetFilteredOrders(userId, filterOptions);

            var response = new ApiResponseDto<List<OrderDetailsResponseDto>>
            {
                IsSuccess = true,
                Message = SuccessMessages.OrderDetailsFetchedSuccessfully,
                Data = result
            };

            return Ok(response);
        }
    }
}
