using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNet_Assignment.Models.Entities
{
    public class RestaurantOwner
    {
        public DateTime CreatedAt { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid UserId { get; set; }

        public virtual User User { get; set; }

        [Key]
        [Column(Order = 2)]
        public Guid RestaurantId { get; set; }

        public virtual Restaurant Restaurant { get; set; }

    }
}