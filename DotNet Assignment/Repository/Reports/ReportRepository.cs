using DotNet_Assignment.Data;
using DotNet_Assignment.Models.DTO;
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
        public async Task<List<TopOrderedItemsResponseDto>> GetTop10OrderedItemsAsync(Guid ownerId, string category, ExcludedItemsDto excludedItemsDto)
        {
            var safeExcludedItems = excludedItemsDto.ExcludeItems ?? new List<string>();
            var safeExcludedRestaurant = excludedItemsDto.ExcludeRestaurants ?? new List<string>();

            string targetCategory = category?.ToLower() ?? string.Empty ;

            return await _context.OrderedItems
                .Where(x => x.Order.Restaurant.RestaurantOwners.Any(ro => ro.UserId == ownerId)
                && x.Order.Status != OrderStatus.Cancelled
                && targetCategory != x.MenuItem.Category
                && !safeExcludedItems.Contains(x.MenuItem.Name)
                && !safeExcludedRestaurant.Contains(x.MenuItem.Restaurant.Name))
                .GroupBy(x => new
                {
                    ItemName = x.MenuItem.Name,
                    RestaurantName = x.MenuItem.Restaurant.Name
                })
                .Select(x => new TopOrderedItemsResponseDto
                {
                    MenuItemName = x.Key.ItemName,
                    TotalQuantity = x.Sum(q => q.Quantity),
                    RestaurantName = x.Key.RestaurantName 
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(10)
                .ToListAsync();
        }

        /// <summary>
        /// Gets two frequently bought together items
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns>list of frequently bought together items</returns>
        public async Task<List<FrequentlyBoughtItemsDto>> GetFrequentlyBoughtTogetherAsync(Guid ownerId, int size, IncludedRestaurantsDto includedRestaurants)
        {
            var safeIncludedRestaurant = (includedRestaurants.IncludeRestaurants ?? new List<string>()).Where(r => !string.IsNullOrWhiteSpace(r)).ToList();
            bool hasRestaurantFilter = safeIncludedRestaurant.Any();

            return await _context.OrderedItems
            .Join(_context.OrderedItems,
                o1 => o1.OrderId,
                o2 => o2.OrderId,
                (o1, o2) => new { o1, o2 })
            .Where(x => x.o1.MenuItemId.CompareTo(x.o2.MenuItemId) < 0)
            .Join(_context.MenuItems,
                x => x.o1.MenuItemId,
                a => a.MenuItemId,
                (x, a) => new { x.o1, x.o2, a })
            .Join(_context.MenuItems,
                x => x.o2.MenuItemId,
                b => b.MenuItemId,
                (x, b) => new { x.a, b })
            .Where(x =>
                !hasRestaurantFilter ||
                (safeIncludedRestaurant.Contains(x.a.Restaurant.Name) &&
                 safeIncludedRestaurant.Contains(x.b.Restaurant.Name)))
            .GroupBy(
                x => new { ItemA = x.a.Name, ItemB = x.b.Name }) 
            .OrderByDescending(g => g.Count())
            .Select(g => new FrequentlyBoughtItemsDto
            {
                Item1 = g.Key.ItemA,
                Item2 = g.Key.ItemB,
                TotalTimesBought = g.Count()
            })
            .Take(size)
            .ToListAsync();
        }
    }
}
