using DotNet_Assignment.Constants;
using System.ComponentModel.DataAnnotations;

namespace DotNet_Assignment.Models.DTO
{
    public class AddRestaurantDto
    {
        [Required]
        [StringLength(100), MinLength(3)]
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
        [RegularExpression(Regex.PincodeRegex)]
        public string Pincode { get; set; }

        [Required]
        public string UserEmail { get; set; }
    }
}
