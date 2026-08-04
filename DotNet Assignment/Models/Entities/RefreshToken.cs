using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DotNet_Assignment.Models.Entities
{
    public class RefreshToken
    {
        [Key]
        public Guid RefreshTokenId { get; set; }

        public string Token {  get; set; }

        public DateTime ExpiresAt { get; set; } 

        public DateTime CreatedAt { get; set; }

        public Guid UserId { get; set; }

        public virtual User User { get; set; }
    }
}
