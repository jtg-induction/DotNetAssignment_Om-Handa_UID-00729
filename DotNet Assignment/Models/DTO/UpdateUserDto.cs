using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DotNet_Assignment.Models.DTO
{
    public class UpdateUserDto
    {
        [Required]
        [StringLength(100), MinLength(2)]
        public string Name { get; set; }

        [Phone]
        [Required]
        [StringLength(10)]
        public string PhoneNumber { get; set; }
    }
}