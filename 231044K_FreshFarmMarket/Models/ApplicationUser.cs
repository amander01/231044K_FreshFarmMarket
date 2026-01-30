using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace _231044K_FreshFarmMarket.Models
{
    public class ApplicationUser : IdentityUser
    {
        // ===============================
        // Registration / Profile fields
        // ===============================

        [Required, StringLength(80)]
        public string FullName { get; set; } = "";

        // Allowed values: "Male", "Female", "NA"
        [Required, StringLength(10)]
        public string Gender { get; set; } = "";

        [Required, StringLength(20)]
        public string MobileNo { get; set; } = "";

        [Required, StringLength(200)]
        public string DeliveryAddress { get; set; } = "";

        [Required]
        public string EncryptedCreditCard { get; set; } = "";

        [StringLength(300)]
        public string? PhotoPath { get; set; }

        [StringLength(800)]
        public string? AboutMe { get; set; }

        // ===============================
        // ✅ Password Age Policy fields
        // ===============================

        // Tracks when password was last changed (UTC)
        public DateTime PasswordLastChangedUtc { get; set; } = DateTime.UtcNow;

        // Forces user to change password at next login (max age exceeded)
        public bool MustChangePassword { get; set; } = false;
    }
}
