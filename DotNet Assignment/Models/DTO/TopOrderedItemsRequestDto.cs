using System;
using System.Collections.Generic;

namespace DotNet_Assignment.Models.DTO
{
    public class TopOrderedItemsRequestDto
    {
        public List<Guid> ExcludedItemIds { get; set; }
        public List<Guid> RestaurantIds { get; set; }
    }
}