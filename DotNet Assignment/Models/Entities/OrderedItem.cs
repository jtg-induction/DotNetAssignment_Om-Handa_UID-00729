using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNet_Assignment.Models.Entities
{
    public class OrderedItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid OrderedItemId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public decimal ItemPrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid OrderId { get; set; }

        public virtual Order Order { get; set; }

        public Guid MenuItemId { get; set; }

        public virtual MenuItem MenuItem { get; set; }
    }
}
