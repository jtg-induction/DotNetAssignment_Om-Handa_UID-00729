using DotNet_Assignment.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNet_Assignment.Models.Entities
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserId { get; set; }

        [Required]
        [EmailAddress]
        [Index("IX_User_Email", IsUnique = true)]
        [StringLength(255)]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
        public string Email { get; set; }

        [Required]
        [StringLength(255), MinLength(8)]
        public string Password { get; set; }

        [Required]
        [StringLength(100), MinLength(2)]
        public string Name { get; set; }

        [Required]
        [StringLength(10), MinLength(10)]
        [RegularExpression(@"^[0-9]{10}$")]
        public string PhoneNumber { get; set; }

        [Range(0, int.MaxValue)]
        public decimal Balance { get; set; } = 1000m;

        public UserRoles Role { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }= DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<UserAddress> UserAddresses { get; set; }= new HashSet<UserAddress>();

        public virtual ICollection<Order> Orders { get; set; }= new HashSet<Order>();

        public virtual ICollection<RestaurantOwner> RestaurantOwners { get; set; } = new HashSet<RestaurantOwner>();

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }

    }
}
