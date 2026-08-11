using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DotNet_Assignment.Models.Entities
{
    public class RefreshToken
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RefreshTokenId { get; set; }

        public string Token {  get; set; }

        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(7);

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid UserId { get; set; }

        public virtual User User { get; set; }
    }
}
