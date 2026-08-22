using System;

namespace DotNet_Assignment.Models.DTO
{
    public class FilterOptionsDto
    {
        public string Category { get; set; } = null;
        public string Status { get; set; } = null;
        public string SortBy { get; set; } = "date";
        public string SortOrder { get; set; } = "desc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid? SearchByOrderId { get; set; }
    }
}
