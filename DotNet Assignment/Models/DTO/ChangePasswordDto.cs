using DotNet_Assignment.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DotNet_Assignment.Models.DTO
{
    public class ChangePasswordDto
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(50), MinLength(8)]
        [RegularExpression(Regex.PasswordRegex, ErrorMessage = ExceptionMessages.WeakPassword)]
        public string NewPassword { get; set; }
    }
}
