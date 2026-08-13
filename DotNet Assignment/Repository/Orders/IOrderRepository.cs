using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Assignment.Repository.Orders
{
    public interface IOrderRepository
    {
        void AddOrder(Order order);

        void AddOrderItem(OrderedItem orderedItem);

        Task<Order> GetOrderByIdAsync(Guid orderId);

        Task<List<OrderedItem>> GetOrderItemsAsync(Guid orderId);

        Task<Order> GetOrderForUpdateAsync(Guid orderId);
    }
}
