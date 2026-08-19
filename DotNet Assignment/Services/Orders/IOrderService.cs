using DotNet_Assignment.Models.DTO;
using System;
using System.Threading.Tasks;

namespace DotNet_Assignment.Services.Orders
{
    public interface IOrderService
    {
        Task<OrderResponseDto> PlaceOrderAsync(Guid userId, OrderRequestDto orderRequestDto);

        Task<OrderDetailsResponseDto> GetOrderDetailsAsync(Guid orderId, Guid userId);

        Task CancelOrderAsync(Guid orderId, Guid userId);
    }
}
