using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNet_Assignment.Models.DTO
{
    public class TopOrderedItemsResponseDto
    {
        public string MenuItemName { get; set; }
        public int TotalQuantityOrdered { get; set; }
        public string RestaurantName { get; set; }
    }
}
