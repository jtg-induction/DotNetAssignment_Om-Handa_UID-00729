using DotNet_Assignment.Constants;
using System.ComponentModel.DataAnnotations;

namespace DotNet_Assignment.Models.DTO
{
    public class UpdateUserDto
    {
        [StringLength(100), MinLength(2)]
        public string Name { get; set; }

        [Phone]
        [StringLength(10)]
        [RegularExpression(Regex.PhoneNumberRegex, ErrorMessage = ExceptionMessages.InvalidPhoneNumber)]
        public string PhoneNumber { get; set; }
    }
}
