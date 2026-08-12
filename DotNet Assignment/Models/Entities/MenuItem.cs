using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNet_Assignment.Models.Entities
{
    public class MenuItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid MenuItemId { get; set; }

        [Required]
        [StringLength(100), MinLength(2)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public decimal Rating { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        [Range (0, int.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int QuantityAvailable { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool InStock { get; set; } = true;

        public Guid RestaurantId { get; set; }

        public virtual Restaurant Restaurant { get; set; }

        public virtual ICollection<OrderedItem> OrderedItems {  get; set; } = new HashSet<OrderedItem>();
    }
}
