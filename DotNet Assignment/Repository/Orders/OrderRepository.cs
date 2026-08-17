using DotNet_Assignment.Data;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Models.Enums;
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
            return await _context.OrderedItems.Include(oi => oi.MenuItem).Where(oi => oi.OrderId == orderId).ToListAsync();
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

        public async Task<List<Order>> FilterOrderAsync(Guid userId, FilterOptionsDto filterOptionsDto)
        {
            var query = _context.Orders.Where(o => o.Restaurant.RestaurantOwners.Any(ro => ro.UserId == userId));

            if (!string.IsNullOrWhiteSpace(filterOptionsDto.status))
            {
                if(Enum.TryParse<OrderStatus>(filterOptionsDto.status, true, out OrderStatus status))
                {
                    query = query.Where(x => x.Status == status);
                }
            }

            if (!string.IsNullOrWhiteSpace(filterOptionsDto.category))
            {
                query = query.Where(x => x.OrderedItems.Any(oi => oi.MenuItem.Category == filterOptionsDto.category));
            }

            if (filterOptionsDto.SearchByOrderId !=null)
            {
                query = query.Where(x => x.OrderId == filterOptionsDto.SearchByOrderId);
            }

            if (!string.IsNullOrWhiteSpace(filterOptionsDto.SortBy))
            {
                if (filterOptionsDto.SortBy.ToLower() == "price")
                {
                    query = filterOptionsDto.SortOrder == "asc"
                            ? query.OrderBy(x => x.TotalPrice)
                            : query.OrderByDescending(x => x.TotalPrice);
                }

                if (filterOptionsDto.SortBy.ToLower() == "date")
                {
                    query = filterOptionsDto.SortOrder == "asc"
                            ? query.OrderBy(x => x.CreatedAt)
                            : query.OrderByDescending(x => x.CreatedAt);
                }
            }

            return await query.Skip((filterOptionsDto.Page - 1) * filterOptionsDto.PageSize)
                .Take(filterOptionsDto.PageSize)
                .ToListAsync();
        }
    }
}
