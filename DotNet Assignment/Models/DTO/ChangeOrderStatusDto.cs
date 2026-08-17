using DotNet_Assignment.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DotNet_Assignment.Models.DTO
{
    public class ChangeOrderStatusDto
    {
        [Required]
        public OrderStatus? Status { get; set; }
    }
}
