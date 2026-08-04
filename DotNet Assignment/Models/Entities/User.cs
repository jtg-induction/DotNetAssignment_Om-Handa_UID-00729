using DotNet_Assignment.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNet_Assignment.Models.Entities
{
    public class User
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        [Index("IX_User_Email", IsUnique = true)]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(255), MinLength(8)]
        public string Password { get; set; }

        [Required]
        [StringLength(100), MinLength(2)]
        public string Name { get; set; }

        [Phone]
        [Required]
        [StringLength(10)]
        public string PhoneNumber { get; set; }

        public decimal Balance { get; set; }

        public UserRoles Role { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<UserAddress> UserAddresses { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public virtual ICollection<RestaurantOwner> RestaurantOwners { get; set; }


    }
}