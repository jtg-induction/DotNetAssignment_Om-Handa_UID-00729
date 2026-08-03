using System;
using System.ComponentModel.DataAnnotations;

namespace DotNet_Assignment.Models.Entities
{
    public class OrderedItem
    {
        [Key]
        public Guid Id { get; set; }

        public int Quantity { get; set; }

        public decimal ItemPrice { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid OrderId { get; set; }

        public virtual Order Order { get; set; }

        public Guid MenuItemId { get; set; }

        public virtual MenuItem MenuItem { get; set; }
    }
}