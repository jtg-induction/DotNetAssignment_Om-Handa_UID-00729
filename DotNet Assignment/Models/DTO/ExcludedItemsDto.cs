using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNet_Assignment.Models.DTO
{
    public class ExcludedItemsDto
    {
        public List<string> ExcludeItems { get; set; }
        public List<string> ExcludeRestaurants { get; set; }
    }
}