using System;
using System.ComponentModel.DataAnnotations;

namespace DotNet_Assignment.Models.Entities
{
    public class UserAddress
    {
        [Key]
        public Guid Id { get; set; }

        public string HouseNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string Street {  get; set; }

        public string Landmark { get; set; }
        
        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        [StringLength(6)]
        public string Pincode { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Guid UserId { get; set; }

        public virtual User User { get; set; }
    }
}