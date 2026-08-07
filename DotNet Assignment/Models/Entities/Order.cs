using DotNet_Assignment.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotNet_Assignment.Models.Entities
{
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; } = Guid.NewGuid();

        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; }= OrderStatus.Placed;

        [Required]
        public string DeliveryAddress { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Guid UserId { get; set; }

        public virtual User User { get; set; }

        public Guid RestaurantId { get; set; }

        public virtual Restaurant Restaurant { get; set; }

        public virtual ICollection<OrderedItem> OrderedItems { get; set; } = new HashSet<OrderedItem>();
    }
}
