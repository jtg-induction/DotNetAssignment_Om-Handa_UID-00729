using System;
using System.Collections.Generic;

namespace DotNet_Assignment.Models.DTO
{
    public class TopOrderedItemsRequestDto
    {
        public List<Guid> ExcludeItems { get; set; }
        public List<Guid> IncludeRestaurants { get; set; }
    }
}