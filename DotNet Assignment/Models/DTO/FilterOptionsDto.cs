using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNet_Assignment.Models.DTO
{
    public class FilterOptionsDto
    {
        public string category { get; set; }
        public string status { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public Guid? SearchByOrderId { get; set; }
    }
}
