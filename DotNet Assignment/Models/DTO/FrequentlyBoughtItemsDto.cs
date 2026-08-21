using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Web;

namespace DotNet_Assignment.Models.DTO
{
    public class FrequentlyBoughtItemsDto
    {
        public string Item1 { get; set; }
        public string Item2 { get; set; }
        public int TotalTimesBought { get; set; }
    }
}