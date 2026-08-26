using DotNet_Assignment.Data;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet_Assignment.Repository.Reports
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets top 10 ordered Items
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns>list of top 10 ordered Items</returns>
        public async Task<List<TopOrderedItemsResponseDto>> GetTop10OrderedItemsAsync(Guid ownerId, string category, TopOrderedItemsRequestDto topOrderedItemsRequest)
        {
            IQueryable<OrderedItem> query = _context.OrderedItems;

            query = query.Where(oi => oi.Order.Restaurant.RestaurantOwners
                    .Any(ro => ro.UserId == ownerId)
                    && oi.Order.Status == OrderStatus.Delivered
                    );

            if (!string.IsNullOrWhiteSpace(category))
            {
                string targetCategory = category.Trim().ToLower();
                query = query.Where(oi => oi.MenuItem.Category.ToLower() == targetCategory);
            }

            if (topOrderedItemsRequest?.ExcludedItemIds != null && topOrderedItemsRequest.ExcludedItemIds.Any())
            {
                var safeExcludedItems = topOrderedItemsRequest.ExcludedItemIds;
                query = query.Where(x => !safeExcludedItems.Contains(x.MenuItem.MenuItemId));
            }

            if (topOrderedItemsRequest?.RestaurantIds != null && topOrderedItemsRequest.RestaurantIds.Any())
            {
                var safeIncludedRestaurants = topOrderedItemsRequest.RestaurantIds;
                query = query.Where(x => safeIncludedRestaurants.Contains(x.MenuItem.RestaurantId));
            }

            return await query
                .GroupBy(oi => new
                {
                    MenuItemId = oi.MenuItemId,
                    RestaurantId = oi.MenuItem.Restaurant.RestaurantId,
                    ItemName = oi.MenuItem.Name,
                    RestaurantName = oi.MenuItem.Restaurant.Name
                })
                .Select(grp => new TopOrderedItemsResponseDto
                {
                    MenuItemName = grp.Key.ItemName,
                    RestaurantName = grp.Key.RestaurantName,
                    TotalQuantityOrdered = grp.Sum(q => q.Quantity)
                })
                .OrderByDescending(oi => oi.TotalQuantityOrdered)
                .Take(10)
                .ToListAsync();

        }

        /// <summary>
        /// Gets two frequently bought together items
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns>list of frequently bought together items</returns>
        public async Task<List<FrequentlyBoughtItemsDto>> GetFrequentlyBoughtTogetherAsync(Guid ownerId, IncludedRestaurantsDto includedRestaurants, int? size)
        {
            int listSize = size ?? 5;

            IQueryable<OrderedItem> baseQuery = _context.OrderedItems;

            var query = baseQuery.SelectMany(o1 => o1.Order.OrderedItems, (o1, o2) => new { ItemA = o1, ItemB = o2 });

            query = query.Where(x => x.ItemA.MenuItemId.CompareTo(x.ItemB.MenuItemId) < 0);

            if (includedRestaurants?.RestaurantsIds != null && includedRestaurants.RestaurantsIds.Any())
            {
                query = query.Where(x =>
                    includedRestaurants.RestaurantsIds.Contains(x.ItemA.MenuItem.RestaurantId) &&
                    includedRestaurants.RestaurantsIds.Contains(x.ItemB.MenuItem.RestaurantId));
            }

            return await query
                .GroupBy(x => new
                {
                    ItemAId = x.ItemA.MenuItemId,
                    ItemBId = x.ItemB.MenuItemId,
                    RestaurantId = x.ItemA.MenuItem.RestaurantId,
                    ItemAName = x.ItemA.MenuItem.Name,
                    ItemBName = x.ItemA.MenuItem.Name,
                    RestaurantName = x.ItemA.MenuItem.Restaurant.Name
                })
                .OrderByDescending(g => g.Count())
                .Select(g => new FrequentlyBoughtItemsDto
                {
                    Item1 = g.Key.ItemAName,
                    Item2 = g.Key.ItemBName,
                    TotalTimesBoughtTogether = g.Count(),
                    RestaurantName = g.Key.RestaurantName
                })
                .Take(listSize)
                .ToListAsync();
        }
    }
}
