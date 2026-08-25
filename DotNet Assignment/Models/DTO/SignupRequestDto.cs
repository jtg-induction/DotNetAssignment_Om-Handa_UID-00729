using DotNet_Assignment.Constants;
using DotNet_Assignment.Models.Enums;
using System.ComponentModel.DataAnnotations;
namespace DotNet_Assignment.Models.DTO
{
    public class SignupRequestDto
    {
        [Required]
        [RegularExpression(Regex.EmailRegex, ErrorMessage = ExceptionMessages.InvalidEmail)]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(50), MinLength(8)]
        [RegularExpression(Regex.PasswordRegex, ErrorMessage =ExceptionMessages.WeakPassword)]
        public string Password { get; set; }

        [Required]
        [StringLength(100), MinLength(2)]
        public string Name { get; set; }

        [Phone]
        [Required]
        [StringLength(10)]
        [RegularExpression(Regex.PhoneNumberRegex, ErrorMessage = ExceptionMessages.InvalidPhoneNumber)]
        public string PhoneNumber { get; set; }

    }
}
