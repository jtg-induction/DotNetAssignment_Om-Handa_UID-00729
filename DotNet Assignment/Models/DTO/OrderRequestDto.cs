using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotNet_Assignment.Models.DTO
{
    public class OrderRequestDto
    {
        [Required]
        public Guid AddressId { get; set; }

        [Required]
        public List<OrderItemDto> OrderedItems { get; set; }

        [Required]
        public Guid RestaurantId { get; set; }
    }
}
