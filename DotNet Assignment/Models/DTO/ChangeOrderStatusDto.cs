using DotNet_Assignment.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNet_Assignment.Models.DTO
{
    public class ChangeOrderStatusDto
    {
        public Guid OrderId { get; set; }

        public OrderStatus Status { get; set; }
    }
}
