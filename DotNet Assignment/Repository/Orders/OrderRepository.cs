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
            return await _context.Orders.SingleOrDefaultAsync(o => o.OrderId == orderId);
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
            var order = await _context.Orders.SqlQuery("SELECT * FROM Orders WITH (UPDLOCK, ROWLOCK) WHERE OrderId = @p0", orderId).SingleOrDefaultAsync();

            if (order != null)
            {
                await _context.Entry(order).Collection(o => o.OrderedItems).LoadAsync();

                await _context.Entry(order).Reference(o => o.Restaurant).LoadAsync();

                if(order.Restaurant != null)
                {
                    await _context.Entry(order.Restaurant).Collection(o => o.RestaurantOwners).LoadAsync();

                }
            }

            return order;
        }

        /// <summary>
        /// Filters Orders based on query from controller
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="filterOptionsDto"></param>
        /// <returns>List of filtered Orders</returns>
        public async Task<List<OrderDetailsResponseDto>> FilterOrderAsync(Guid userId, FilterOptionsDto filterOptionsDto)
        {
            var query = _context.Orders.AsNoTracking().Where(o => o.Restaurant.RestaurantOwners.Any(ro => ro.UserId == userId));

            if (filterOptionsDto == null)
            {
                filterOptionsDto = new FilterOptionsDto();
            }

            if (!string.IsNullOrWhiteSpace(filterOptionsDto.Status))
            {
                if(Enum.TryParse<OrderStatus>(filterOptionsDto.Status, true, out OrderStatus status))
                {
                    query = query.Where(x => x.Status == status);
                }
            }

            if (!string.IsNullOrWhiteSpace(filterOptionsDto.Category))
            {
                query = query.Where(x => x.OrderedItems.Any(oi => oi.MenuItem.Category == filterOptionsDto.Category));
            }

            if (filterOptionsDto.SearchByOrderId !=null)
            {
                query = query.Where(x => x.OrderId == filterOptionsDto.SearchByOrderId);
            }

            if (!string.IsNullOrWhiteSpace(filterOptionsDto.SortBy))
            {
                if (filterOptionsDto.SortBy.Equals("price", StringComparison.OrdinalIgnoreCase))
                {
                    query = filterOptionsDto.SortOrder == "asc"
                            ? query.OrderBy(x => x.TotalPrice)
                            : query.OrderByDescending(x => x.TotalPrice);
                }

                if (filterOptionsDto.SortBy.Equals("date", StringComparison.OrdinalIgnoreCase))
                {
                    query = filterOptionsDto.SortOrder == "asc"
                            ? query.OrderBy(x => x.CreatedAt)
                            : query.OrderByDescending(x => x.CreatedAt);
                }
            }

            return await query.Skip((filterOptionsDto.Page - 1) * filterOptionsDto.PageSize)
                .Take(filterOptionsDto.PageSize).Select(o => new OrderDetailsResponseDto
                {
                    OrderId = o.OrderId,
                    TotalPrice = o.TotalPrice,
                    Status = o.Status.ToString(),
                    DeliveryAddress = o.DeliveryAddress,
                    RestaurantName = o.Restaurant.Name,

                    OrderedItems = o.OrderedItems.Select(oi => new MenuDetailsResponseDto
                    {
                        Name = oi.MenuItem.Name,
                        Description = oi.MenuItem.Description,
                        Price = oi.ItemPrice,
                        Quantity = oi.Quantity
                    }).ToList()
                }).ToListAsync();
        }
    }
}
