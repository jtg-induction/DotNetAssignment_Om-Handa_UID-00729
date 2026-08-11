using DotNet_Assignment.Models.Enums;
using System.ComponentModel.DataAnnotations;
namespace DotNet_Assignment.Models.DTO
{
    public class SignupRequestDto
    {
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(50), MinLength(8)]
        public string Password { get; set; }

        [Required]
        [StringLength(100), MinLength(2)]
        public string Name { get; set; }

        [Phone]
        [Required]
        [StringLength(10)]
        public string PhoneNumber { get; set; }

        public UserRoles Role { get; set; }
    }
}
