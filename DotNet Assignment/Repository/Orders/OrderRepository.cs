using DotNet_Assignment.Data;
using DotNet_Assignment.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet_Assignment.Repository.Orders
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds order to DB context
        /// </summary>
        /// <param name="order"></param>
        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
        }

        /// <summary>
        /// Adds ordered Item to DB context
        /// </summary>
        /// <param name="orderedItem"></param>
        public void AddOrderItem(OrderedItem orderedItem)
        {
            _context.OrderedItems.Add(orderedItem);
        }

        /// <summary>
        /// Gets an order by its ID
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>Order</returns>
        public async Task<Order> GetOrderByIdAsync(Guid orderId)
        {
            return _context.Orders.SingleOrDefault(o => o.OrderId == orderId);
        }

        /// <summary>
        /// Gets list of ordered Items using order ID
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>List of ordered items</returns>
        public async Task<List<OrderedItem>> GetOrderItemsAsync(Guid orderId)
        {
            return await _context.OrderedItems.Include(oi=> oi.MenuItem).Where(oi => oi.OrderId == orderId).ToListAsync();
        }
        
        /// <summary>
        /// Applies update and row lock and gets order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>order</returns>
        public async Task<Order> GetOrderForUpdateAsync(Guid orderId)
        {
            return await _context.Orders.SqlQuery("SELECT * FROM  Orders WITH (UPDLOCK, ROWLOCK) where OrderId = @p0", orderId).SingleOrDefaultAsync();
        }
    }
}
