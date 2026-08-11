using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNet_Assignment.Models.Entities
{
    public class UserAddress
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserAddressId { get; set; }

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
        [RegularExpression(@"^[1-9][0-9]{5}$")]
        public string Pincode { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Guid UserId { get; set; }

        public virtual User User { get; set; }
    }
}
