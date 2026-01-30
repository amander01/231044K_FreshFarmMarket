using System;
using System.ComponentModel.DataAnnotations;

namespace _231044K_FreshFarmMarket.Models
{
    public class PasswordHistory
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = "";

        [Required]
        public string PasswordHash { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
