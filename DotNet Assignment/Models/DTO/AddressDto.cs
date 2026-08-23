using DotNet_Assignment.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DotNet_Assignment.Models.DTO
{
    public class AddressDto
    {
        public string HouseNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string Street { get; set; }

        public string Landmark { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        [StringLength(6)]
        [RegularExpression(Regex.PincodeRegex, ErrorMessage = ExceptionMessages.InvalidPincode)]
        public string Pincode { get; set; }

    }
}
