using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotNet_Assignment.Models.Entities
{
    public class Restaurant
    {
        public Guid RestaurantId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)] 
        [MinLength(3)]
        public string Name { get; set; }

        [Required] 
        [MaxLength(100)]
        public string Description { get; set; }

        [Required]
        [MaxLength(255)]
        public string Street { get; set; }

        [MaxLength(255)]
        public string Landmark { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        [StringLength(6)]
        public string Pincode { get; set; }

        public decimal Rating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsOpen { get; set; } = true;

        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();

        public virtual ICollection<MenuItem> MenuItems { get; set; } = new HashSet<MenuItem>();

        public virtual ICollection<RestaurantOwner> RestaurantOwners { get; set; } = new HashSet<RestaurantOwner>();

    }
}
