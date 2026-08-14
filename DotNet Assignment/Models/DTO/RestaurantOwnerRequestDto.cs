using System;
using System.ComponentModel.DataAnnotations;

namespace DotNet_Assignment.Models.DTO
{
    public class RestaurantOwnerRequestDto
    {
        [Required]
        public string UserEmail { get; set; }

        [Required]
        public Guid RestaurantId { get; set; }
    }
}