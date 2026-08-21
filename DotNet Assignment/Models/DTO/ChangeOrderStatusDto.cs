using DotNet_Assignment.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace DotNet_Assignment.Models.DTO
{
    public class ChangeOrderStatusDto
    {
        [Required]
        public OrderStatus? Status { get; set; }
    }
}
