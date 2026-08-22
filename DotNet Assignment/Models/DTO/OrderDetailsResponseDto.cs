using System;
using System.Collections.Generic;

namespace DotNet_Assignment.Models.DTO
{
    public class OrderDetailsResponseDto
    {
        public Guid OrderId { get; set; }

        public decimal TotalPrice { get; set; }

        public string Status { get; set; }

        public string DeliveryAddress { get; set; }

        public string RestaurantName { get; set; }
        
        public List<MenuDetailsResponseDto> OrderedItems { get; set; }
    }
}
