using DotNet_Assignment.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotNet_Assignment.Models.Entities
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        public decimal Total_Price { get; set; }

        public OrderStatus Status { get; set; }

        public string DeliveryAddress { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Guid UserId { get; set; }

        public virtual User User { get; set; }

        public Guid RestaurantId { get; set; }

        public virtual Restaurant Restaurant { get; set; }

        public virtual ICollection<OrderedItem> OrderedItems { get; set; }
    }
}