using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DotNet_Assignment.Models.Entities
{
    public class RefreshToken
    {
        public Guid RefreshTokenId { get; set; } = Guid.NewGuid();

        public string Token {  get; set; }

        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(7);

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid UserId { get; set; }

        public virtual User User { get; set; }
    }
}
