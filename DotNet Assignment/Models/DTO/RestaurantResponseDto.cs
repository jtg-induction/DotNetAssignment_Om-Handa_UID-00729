using System;

namespace DotNet_Assignment.Models.DTO
{
    public class RestaurantResponseDto
    {
        public Guid RestaurantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Street { get; set; }
        public string Landmark { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Pincode { get; set; }
        public decimal Rating { get; set; }
        public bool IsOpen { get; set; } = true;
    }
}
