using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNet_Assignment.Models.DTO
{
    public class TopOrderedItemsResponseDto
    {
        public string MenuItemName { get; set; }
        public int TotalQuantity { get; set; }
    }
}
